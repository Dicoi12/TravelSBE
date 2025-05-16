import requests
import matplotlib.pyplot as plt
import seaborn as sns
import sys
import urllib3
import json

# Dezactivează avertismentele pentru certificatul SSL auto-semnat
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

def get_data_from_api():
    try:
        url = 'https://localhost:7100/api/ML/visualization'
        print(f"Încerc să conectez la {url}")
        
        response = requests.get(url, verify=False)
        response.raise_for_status()
        
        data = response.json()
        if not isinstance(data, dict) or 'result' not in data:
            print("Eroare: Datele nu sunt în formatul așteptat")
            sys.exit(1)
            
        objectives = data['result']
        if not objectives:
            print("Eroare: Lista de obiective este goală")
            sys.exit(1)
            
        return objectives
    except Exception as e:
        print(f"Eroare la obținerea datelor: {e}")
        sys.exit(1)

try:
    data = get_data_from_api()
    
    plt.figure(figsize=(15, 10))
    
    # Plot punctele cu culori diferite pentru fiecare cluster
    scatter = plt.scatter([d['x'] for d in data], 
                         [d['y'] for d in data], 
                         c=[d['clusterId'] for d in data], 
                         cmap='viridis',
                         s=100)

    # Adaugă etichete pentru fiecare punct
    for point in data:
        plt.text(point['x'], point['y'], 
                str(point['id']), 
                fontsize=8,
                ha='center',
                va='center',
                color='white',
                bbox=dict(facecolor='black', alpha=0.5, edgecolor='none', pad=1))

    plt.title('Clustering pe Hartă cu ID-uri Obiective')
    plt.xlabel('Longitudine')
    plt.ylabel('Latitudine')
    plt.colorbar(scatter, label='Cluster ID')
    plt.gca().set_aspect('equal')
    
    plt.tight_layout()
    plt.savefig('clustering_analysis.png', dpi=300, bbox_inches='tight')
    print("Figura a fost salvată ca 'clustering_analysis.png'")
    plt.close()

except Exception as e:
    print(f"Eroare la generarea figurii: {e}")
    import traceback
    traceback.print_exc()
    sys.exit(1) 