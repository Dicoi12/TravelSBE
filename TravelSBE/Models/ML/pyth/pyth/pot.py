import requests
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
import seaborn as sns
import sys
import urllib3
import json
import numpy as np

# Dezactivează avertismentele pentru certificatul SSL auto-semnat
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

def get_data_from_api():
    try:
        url = 'https://localhost:7100/api/ML/visualization'
        print(f"Încerc să conectez la {url}")
        
        response = requests.get(url, verify=False)
        response.raise_for_status()
        
        data = response.json()
        print("\nDate primite de la API:")
        print(f"Număr total de obiective: {len(data['result'])}")
        
        # Afișează statistici despre date
        cluster_ids = set(item['clusterId'] for item in data['result'])
        print(f"Număr de clustere găsite: {len(cluster_ids)}")
        print(f"ID-urile clusterelor: {cluster_ids}")
        
        # Afișează distribuția obiectivelor pe clustere
        cluster_distribution = {}
        for item in data['result']:
            cluster_id = item['clusterId']
            if cluster_id not in cluster_distribution:
                cluster_distribution[cluster_id] = 0
            cluster_distribution[cluster_id] += 1
        
        print("\nDistribuția obiectivelor pe clustere:")
        for cluster_id, count in cluster_distribution.items():
            print(f"Cluster {cluster_id}: {count} obiective")
        
        # Verifică valorile pentru fiecare proprietate
        print("\nStatistici despre proprietăți:")
        properties = ['x', 'y', 'averageRating', 'price']
        for prop in properties:
            values = [item[prop] for item in data['result']]
            print(f"{prop}: min={min(values):.2f}, max={max(values):.2f}, medie={sum(values)/len(values):.2f}")
        
        return data['result']
    except Exception as e:
        print(f"Eroare la obținerea datelor: {e}")
        sys.exit(1)

def create_visualization(data):
    # Setează stilul pentru grafice
    plt.style.use('default')
    
    # Creează o figură cu 2 subplot-uri
    fig = plt.figure(figsize=(20, 10))
    
    # Subplot 1: Vizualizare 3D
    ax1 = fig.add_subplot(121, projection='3d')
    
    # Grupează datele pe clustere
    clusters = {}
    for item in data:
        cluster_id = item['clusterId']
        if cluster_id not in clusters:
            clusters[cluster_id] = []
        clusters[cluster_id].append(item)
    
    # Culori pentru clustere
    colors = plt.cm.viridis(np.linspace(0, 1, len(clusters)))
    
    # Plot pentru fiecare cluster
    for cluster_id, cluster_data in clusters.items():
        x = [d['x'] for d in cluster_data]
        y = [d['y'] for d in cluster_data]
        z = [d['averageRating'] for d in cluster_data]
        sizes = [d['price'] * 10 for d in cluster_data]
        
        scatter = ax1.scatter(x, y, z, 
                            c=[colors[cluster_id] for _ in range(len(cluster_data))],
                            s=sizes,
                            alpha=0.6,
                            label=f'Cluster {cluster_id}')
        
        # Adaugă etichete pentru ID-uri
        for point in cluster_data:
            ax1.text(point['x'], point['y'], point['averageRating'],
                    str(point['id']),
                    fontsize=8)
    
    ax1.set_title('Clustering în Spațiu 3D\n(X: Longitudine, Y: Latitudine, Z: Rating)')
    ax1.set_xlabel('Longitudine')
    ax1.set_ylabel('Latitudine')
    ax1.set_zlabel('Rating Mediu')
    
    # Adaugă legenda
    ax1.legend()
    
    # Subplot 2: Distribuția proprietăților pe clustere
    ax2 = fig.add_subplot(122)
    
    # Pregătește datele pentru boxplot
    cluster_data = {cluster_id: {
        'rating': [d['averageRating'] for d in cluster],
        'price': [d['price'] for d in cluster]
    } for cluster_id, cluster in clusters.items()}
    
    # Creează boxplot-uri pentru rating și preț
    boxplot_data = []
    labels = []
    for cluster_id in clusters.keys():
        boxplot_data.extend([cluster_data[cluster_id]['rating'], cluster_data[cluster_id]['price']])
        labels.extend([f'Cluster {cluster_id}\nRating', f'Cluster {cluster_id}\nPreț'])
    
    ax2.boxplot(boxplot_data, labels=labels)
    ax2.set_title('Distribuția Rating-ului și Prețului pe Clustere')
    ax2.set_ylabel('Valoare')
    plt.xticks(rotation=45)
    
    # Ajustează layout-ul
    plt.tight_layout()
    
    # Salvează figura
    plt.savefig('clustering_analysis.png', dpi=300, bbox_inches='tight')
    print("\nFigura a fost salvată ca 'clustering_analysis.png'")
    
    # Afișează statistici detaliate pentru fiecare cluster
    print("\nStatistici detaliate per cluster:")
    for cluster_id, cluster in clusters.items():
        print(f"\nCluster {cluster_id}:")
        print(f"Număr de obiective: {len(cluster)}")
        print(f"Rating mediu: {np.mean([d['averageRating'] for d in cluster]):.2f}")
        print(f"Preț mediu: {np.mean([d['price'] for d in cluster]):.2f}")
        print(f"Tipuri de obiective: {set(d['typeName'] for d in cluster)}")
        
        # Afișează distribuția geografică
        x_coords = [d['x'] for d in cluster]
        y_coords = [d['y'] for d in cluster]
        print(f"Zonă geografică: X({min(x_coords):.2f} - {max(x_coords):.2f}), Y({min(y_coords):.2f} - {max(y_coords):.2f})")

try:
    # Obține datele de la API
    data = get_data_from_api()
    
    if not data:
        print("Nu s-au primit date de la API")
        sys.exit(1)

    # Creează vizualizarea
    create_visualization(data)

except Exception as e:
    print(f"Eroare la generarea figurii: {e}")
    import traceback
    traceback.print_exc()
    sys.exit(1) 