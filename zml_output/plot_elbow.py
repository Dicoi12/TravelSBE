"""
Elbow Method Visualization for K-Means Clustering
---------------------------------------------------
Reads elbow_data.json produced by MLService and plots the WCSS curve,
highlighting the optimal K chosen by the perpendicular-distance method.

Usage:
    python plot_elbow.py                     # reads elbow_data.json in same folder
    python plot_elbow.py path/to/elbow_data.json
"""

import json
import sys
import os
from pathlib import Path

import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
import numpy as np


def load_data(json_path: str) -> dict:
    with open(json_path, "r") as f:
        return json.load(f)


def plot_elbow(data: dict, save_path: str | None = None) -> None:
    points = data["dataPoints"]
    optimal_k = data["optimalK"]
    timestamp = data.get("timestamp", "")

    ks = [p["k"] for p in points]
    wcss = [p["wcss"] for p in points]

    fig, ax = plt.subplots(figsize=(10, 6))

    # Main WCSS line
    ax.plot(ks, wcss, marker="o", linewidth=2, markersize=7,
            color="#2196F3", label="WCSS (Inertia)")

    # Elbow reference line (first -> last point)
    ax.plot([ks[0], ks[-1]], [wcss[0], wcss[-1]],
            linestyle="--", color="#9E9E9E", linewidth=1, alpha=0.7, label="Reference line")

    # Highlight optimal K
    opt_wcss = wcss[ks.index(optimal_k)]
    ax.scatter([optimal_k], [opt_wcss], s=160, zorder=5,
               color="#F44336", label=f"Optimal K = {optimal_k}")
    ax.axvline(x=optimal_k, linestyle=":", color="#F44336", linewidth=1.5, alpha=0.6)

    # Perpendicular drop lines for every point (visual reference)
    x1, y1 = ks[0], wcss[0]
    x2, y2 = ks[-1], wcss[-1]
    dx, dy = x2 - x1, y2 - y1
    line_len_sq = dx * dx + dy * dy

    for k, w in zip(ks, wcss):
        t = ((k - x1) * dx + (w - y1) * dy) / line_len_sq
        foot_x = x1 + t * dx
        foot_y = y1 + t * dy
        ax.plot([k, foot_x], [w, foot_y], color="#BDBDBD", linewidth=0.8, alpha=0.5)

    # Annotations
    for k, w in zip(ks, wcss):
        ax.annotate(f"{w:.4f}", (k, w),
                    textcoords="offset points", xytext=(0, 10),
                    ha="center", fontsize=8, color="#555555")

    ax.set_xlabel("Number of clusters (K)", fontsize=13)
    ax.set_ylabel("WCSS (Within-Cluster Sum of Squares)", fontsize=13)
    ax.set_title("Elbow Method — Optimal K Selection\n"
                 f"(trained at {timestamp[:19].replace('T', ' ')} UTC, optimal K = {optimal_k})",
                 fontsize=14, pad=14)
    ax.set_xticks(ks)
    ax.legend(fontsize=11)
    ax.grid(axis="y", linestyle="--", alpha=0.4)
    ax.grid(axis="x", linestyle=":", alpha=0.3)

    plt.tight_layout()

    if save_path:
        fig.savefig(save_path, dpi=150, bbox_inches="tight")
        print(f"Plot saved to: {save_path}")
    else:
        plt.show()


if __name__ == "__main__":
    script_dir = Path(__file__).parent
    json_path = sys.argv[1] if len(sys.argv) > 1 else str(script_dir / "elbow_data.json")

    if not os.path.exists(json_path):
        print(f"Error: file not found: {json_path}")
        print("Run the backend at least once so MLService generates elbow_data.json.")
        sys.exit(1)

    data = load_data(json_path)
    print(f"Loaded data: {len(data['dataPoints'])} K values tested, optimal K = {data['optimalK']}")

    # Also save a PNG next to the JSON
    png_path = str(Path(json_path).with_suffix(".png"))
    plot_elbow(data, save_path=png_path)
    plot_elbow(data)   # also open the interactive window
