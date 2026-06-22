"""
K-Means cluster visualization.

Reads cluster_data.json produced by MLService and generates:

1. clusters_normalized_xy.png - normalized X/Y chart for presentation.
2. clusters_geographic.png    - real longitude/latitude chart.
3. clusters_pca.png           - optional PCA projection over all 5 features.

The K-Means labels are calculated by the backend using all normalized and
weighted features: X, Y, rating, price, and type. The normalized X/Y chart is
only the 2D representation used for explaining the result visually.

Usage:
    pip install matplotlib numpy
    pip install scikit-learn       # optional, only for the PCA chart
    python plot_clusters.py
    python plot_clusters.py path/to/cluster_data.json --no-show
"""

import argparse
import json
import os
from pathlib import Path

import matplotlib.patches as mpatches
import matplotlib.pyplot as plt
import numpy as np
from matplotlib.lines import Line2D

SHOW_LABELS = True
SAVE_PNG = True
FIG_DPI = 150
POINT_SIZE = 60
CENTROID_SIZE = 300

PALETTE = [
    "#E53935", "#1E88E5", "#43A047", "#FB8C00", "#8E24AA",
    "#00ACC1", "#F4511E", "#3949AB", "#00897B", "#FFB300",
    "#D81B60", "#6D4C41",
]


def cluster_color(cluster_id: int) -> str:
    return PALETTE[cluster_id % len(PALETTE)]


def load_data(path: str) -> dict:
    with open(path, "r", encoding="utf-8") as file:
        return json.load(file)


def add_legend(fig: plt.Figure, cluster_ids: list[int]) -> None:
    handles = [
        mpatches.Patch(color=cluster_color(cluster_id), label=f"Cluster {cluster_id}")
        for cluster_id in cluster_ids
    ]
    handles.append(
        Line2D(
            [0],
            [0],
            marker="*",
            color="w",
            markerfacecolor="black",
            markersize=12,
            label="Centroid",
        )
    )
    fig.legend(
        handles=handles,
        loc="lower center",
        ncol=min(len(cluster_ids) + 1, 6),
        fontsize=9,
        bbox_to_anchor=(0.5, -0.02),
        frameon=True,
    )


def annotate_point(ax: plt.Axes, name: str, x: float, y: float) -> None:
    if not SHOW_LABELS:
        return

    ax.annotate(
        name,
        (x, y),
        fontsize=5.5,
        alpha=0.7,
        xytext=(3, 3),
        textcoords="offset points",
    )


def plot_normalized_xy(data: dict, ax: plt.Axes) -> None:
    points = data["dataPoints"]
    centroids = data["centroids"]
    cluster_ids = sorted({point["clusterId"] for point in points})
    timestamp = data.get("timestamp", "")

    for cluster_id in cluster_ids:
        cluster_points = [point for point in points if point["clusterId"] == cluster_id]
        xs = [point["normX"] for point in cluster_points]
        ys = [point["normY"] for point in cluster_points]

        ax.scatter(
            xs,
            ys,
            s=POINT_SIZE,
            color=cluster_color(cluster_id),
            alpha=0.75,
            edgecolors="white",
            linewidths=0.4,
            label=f"Cluster {cluster_id} ({len(cluster_points)} obj.)",
        )

        for point in cluster_points:
            annotate_point(ax, point["name"], point["normX"], point["normY"])

    for centroid in centroids:
        ax.scatter(
            centroid["normX"],
            centroid["normY"],
            s=CENTROID_SIZE,
            color=cluster_color(centroid["clusterId"]),
            marker="*",
            edgecolors="black",
            linewidths=0.8,
            zorder=5,
        )

    ax.set_xlabel("Axa X normalizata: longitudine ponderata", fontsize=11)
    ax.set_ylabel("Axa Y normalizata: latitudine ponderata", fontsize=11)
    ax.set_title(
        "Reprezentare normalizata X/Y a clusterelor "
        f"(K = {len(cluster_ids)})\n"
        f"date generate la {timestamp[:19].replace('T', ' ')} UTC",
        fontsize=12,
        pad=10,
    )
    ax.grid(linestyle="--", alpha=0.35)

    max_x = max([point["normX"] for point in points] + [centroid["normX"] for centroid in centroids])
    max_y = max([point["normY"] for point in points] + [centroid["normY"] for centroid in centroids])
    ax.set_xlim(-0.02, max(max_x, 0.6) + 0.02)
    ax.set_ylim(-0.02, max(max_y, 0.6) + 0.02)
    ax.set_aspect("equal", adjustable="box")


def plot_geographic(data: dict, ax: plt.Axes) -> None:
    points = data["dataPoints"]
    centroids = data["centroids"]
    cluster_ids = sorted({point["clusterId"] for point in points})

    for cluster_id in cluster_ids:
        cluster_points = [point for point in points if point["clusterId"] == cluster_id]
        xs = [point["geoX"] for point in cluster_points]
        ys = [point["geoY"] for point in cluster_points]

        ax.scatter(
            xs,
            ys,
            s=POINT_SIZE,
            color=cluster_color(cluster_id),
            alpha=0.75,
            edgecolors="white",
            linewidths=0.4,
            label=f"Cluster {cluster_id} ({len(cluster_points)} obj.)",
        )

        for point in cluster_points:
            annotate_point(ax, point["name"], point["geoX"], point["geoY"])

    for centroid in centroids:
        ax.scatter(
            centroid["geoX"],
            centroid["geoY"],
            s=CENTROID_SIZE,
            color=cluster_color(centroid["clusterId"]),
            marker="*",
            edgecolors="black",
            linewidths=0.8,
            zorder=5,
        )

    ax.set_xlabel("Longitude", fontsize=11)
    ax.set_ylabel("Latitude", fontsize=11)
    ax.set_title(f"Pozitia geografica a clusterelor (K = {len(cluster_ids)})", fontsize=12, pad=10)
    ax.grid(linestyle="--", alpha=0.35)


def plot_pca(data: dict, ax: plt.Axes) -> bool:
    try:
        from sklearn.decomposition import PCA
    except ImportError:
        return False

    points = data["dataPoints"]
    centroids = data["centroids"]

    features = np.array(
        [
            [point["normX"], point["normY"], point["normRating"], point["normPrice"], point["normType"]]
            for point in points
        ]
    )
    labels = np.array([point["clusterId"] for point in points])
    names = [point["name"] for point in points]

    centroid_features = np.array(
        [
            [
                centroid["normX"],
                centroid["normY"],
                centroid["normRating"],
                centroid["normPrice"],
                centroid["normType"],
            ]
            for centroid in centroids
        ]
    )
    centroid_ids = [centroid["clusterId"] for centroid in centroids]

    pca = PCA(n_components=2)
    all_features = np.vstack([features, centroid_features])
    all_2d = pca.fit_transform(all_features)

    points_2d = all_2d[: len(features)]
    centroids_2d = all_2d[len(features) :]
    var1 = pca.explained_variance_ratio_[0] * 100
    var2 = pca.explained_variance_ratio_[1] * 100

    for cluster_id in sorted(set(labels.tolist())):
        mask = labels == cluster_id
        ax.scatter(
            points_2d[mask, 0],
            points_2d[mask, 1],
            s=POINT_SIZE,
            color=cluster_color(cluster_id),
            alpha=0.75,
            edgecolors="white",
            linewidths=0.4,
            label=f"Cluster {cluster_id} ({mask.sum()} obj.)",
        )

        if SHOW_LABELS:
            for index, show in enumerate(mask):
                if show:
                    annotate_point(ax, names[index], points_2d[index, 0], points_2d[index, 1])

    for index, cluster_id in enumerate(centroid_ids):
        ax.scatter(
            centroids_2d[index, 0],
            centroids_2d[index, 1],
            s=CENTROID_SIZE,
            color=cluster_color(cluster_id),
            marker="*",
            edgecolors="black",
            linewidths=0.8,
            zorder=5,
        )

    ax.axhline(0, color="grey", linewidth=0.6, linestyle=":")
    ax.axvline(0, color="grey", linewidth=0.6, linestyle=":")
    ax.set_xlabel(f"PC1 ({var1:.1f}% varianta)", fontsize=11)
    ax.set_ylabel(f"PC2 ({var2:.1f}% varianta)", fontsize=11)
    ax.set_title(
        "Proiectie PCA peste cele 5 proprietati normalizate "
        f"({var1 + var2:.1f}% varianta explicata)",
        fontsize=12,
        pad=10,
    )
    ax.grid(linestyle="--", alpha=0.35)
    return True


def save_figure(fig: plt.Figure, path: Path) -> None:
    if SAVE_PNG:
        fig.savefig(path, dpi=FIG_DPI, bbox_inches="tight")
        print(f"Saved: {path}")


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Generate cluster visualization charts.")
    parser.add_argument(
        "json_path",
        nargs="?",
        default=str(Path(__file__).parent / "cluster_data.json"),
        help="Path to cluster_data.json generated by MLService.",
    )
    parser.add_argument("--no-show", action="store_true", help="Save figures without opening a window.")
    return parser


if __name__ == "__main__":
    args = build_parser().parse_args()
    json_path = Path(args.json_path)

    if not json_path.exists():
        print(f"Error: file not found: {json_path}")
        print("Run the backend at least once so MLService generates cluster_data.json.")
        raise SystemExit(1)

    if args.no_show:
        plt.switch_backend("Agg")

    cluster_data = load_data(str(json_path))
    cluster_ids = sorted({point["clusterId"] for point in cluster_data["dataPoints"]})
    output_dir = json_path.parent

    print(f"Loaded {len(cluster_data['dataPoints'])} objectives in {len(cluster_ids)} clusters.")

    fig_xy, ax_xy = plt.subplots(figsize=(12, 8))
    plot_normalized_xy(cluster_data, ax_xy)
    add_legend(fig_xy, cluster_ids)
    fig_xy.tight_layout(rect=[0, 0.06, 1, 1])
    save_figure(fig_xy, output_dir / "clusters_normalized_xy.png")

    fig_geo, ax_geo = plt.subplots(figsize=(12, 8))
    plot_geographic(cluster_data, ax_geo)
    add_legend(fig_geo, cluster_ids)
    fig_geo.tight_layout(rect=[0, 0.06, 1, 1])
    save_figure(fig_geo, output_dir / "clusters_geographic.png")

    fig_pca, ax_pca = plt.subplots(figsize=(12, 8))
    if plot_pca(cluster_data, ax_pca):
        add_legend(fig_pca, cluster_ids)
        fig_pca.tight_layout(rect=[0, 0.06, 1, 1])
        save_figure(fig_pca, output_dir / "clusters_pca.png")
    else:
        plt.close(fig_pca)
        print("Skipped PCA chart: install scikit-learn to generate clusters_pca.png.")

    if not args.no_show:
        plt.show()
