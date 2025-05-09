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
        
        # Adăugăm verify=False pentru a ignora verificarea certificatului SSL
        response = requests.get(url, verify=False)
        response.raise_for_status()
        
        data = response.json()
        print("Date primite de la API:")
        print(json.dumps(data, indent=2))
        
        # Verificăm dacă avem datele necesare
        if not isinstance(data, dict) or 'result' not in data:
            print("Eroare: Datele nu sunt în formatul așteptat (obiect cu proprietatea 'result')")
            sys.exit(1)
            
        objectives = data['result']
        if not objectives:
            print("Eroare: Lista de obiective este goală")
            sys.exit(1)
            
        # Verificăm dacă toate obiectivele au câmpurile necesare
        required_fields = ['x', 'y', 'clusterId', 'id', 'distanceFromCenter']
        for i, item in enumerate(objectives):
            missing_fields = [field for field in required_fields if field not in item]
            if missing_fields:
                print(f"Eroare: Obiectivul {i} nu are câmpurile: {missing_fields}")
                sys.exit(1)
        
        return objectives
    except requests.exceptions.ConnectionError as e:
        print(f"Eroare de conectare: {e}")
        sys.exit(1)
    except requests.exceptions.HTTPError as e:
        print(f"Eroare HTTP: {e}")
        sys.exit(1)
    except json.JSONDecodeError as e:
        print(f"Eroare la decodarea JSON: {e}")
        print("Răspuns primit:", response.text)
        sys.exit(1)
    except Exception as e:
        print(f"Eroare la obținerea datelor: {e}")
        sys.exit(1)

try:
    # Obține datele de la API
    data = get_data_from_api()
    
    if not data:
        print("Nu s-au primit date de la API")
        sys.exit(1)

    # Creează figura
    plt.figure(figsize=(15, 10))
    
    # Plot: Clustering pe hartă
    scatter = plt.scatter([d['x'] for d in data], [d['y'] for d in data], 
                         c=[d['clusterId'] for d in data], cmap='viridis',
                         s=100)  # Mărim dimensiunea punctelor
    plt.title('Clustering pe Hartă')
    plt.xlabel('Longitudine')
    plt.ylabel('Latitudine')
    plt.gca().set_aspect('equal')

    # Adaugă etichete mici pentru ID-uri
    for point in data:
        plt.annotate(str(point['id']), 
                    (point['x'], point['y']),
                    xytext=(0, 0),
                    textcoords='offset points',
                    ha='center',
                    va='center',
                    fontsize=8,
                    color='white')

    # Adaugă legenda pentru clustere
    plt.colorbar(scatter, label='Cluster ID')

    # Ajustează layout-ul
    plt.tight_layout()

    # Salvează figura
    plt.savefig('clustering_analysis.png', dpi=300, bbox_inches='tight')
    print("Figura a fost salvată ca 'clustering_analysis.png'")
    plt.close()

except Exception as e:
    print(f"Eroare la generarea figurii: {e}")
    import traceback
    traceback.print_exc()
    sys.exit(1) 