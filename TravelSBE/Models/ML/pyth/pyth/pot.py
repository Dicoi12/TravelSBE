import requests
import matplotlib.pyplot as plt
import seaborn as sns
import sys
import urllib3
import json
import numpy as np

# Dezactivează avertismentele pentru certificatul SSL auto-semnat
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

def get_neighbors_from_api(objective_id):
    try:
        url = f'https://localhost:7100/api/ML/neighbors/{objective_id}'
        print(f"Încerc să conectez la {url}")
        
        response = requests.get(url, verify=False)
        response.raise_for_status()
        
        data = response.json()
        print("\nDate primite de la API:")
        print(f"Număr de vecini găsiți: {len(data['result'])}")
        
        return data['result']
    except Exception as e:
        print(f"Eroare la obținerea datelor: {e}")
        sys.exit(1)

def get_objective_details(objective_id):
    try:
        url = f'https://localhost:7100/api/Objective/{objective_id}'
        print(f"Încerc să conectez la {url}")
        
        response = requests.get(url, verify=False)
        response.raise_for_status()
        
        data = response.json()
        return data['result']
    except Exception as e:
        print(f"Eroare la obținerea detaliilor obiectivului: {e}")
        sys.exit(1)

def create_neighbors_visualization(objective_id, neighbors):
    # Setează stilul pentru grafice
    plt.style.use('default')
    
    # Creează figura
    fig, ax = plt.subplots(figsize=(12, 8))
    
    # Obține detaliile obiectivului principal
    main_objective = get_objective_details(objective_id)
    
    # Plot pentru obiectivul principal
    ax.scatter(main_objective['longitude'], main_objective['latitude'], 
              c='red', s=200, label='Obiectiv Principal', marker='*')
    
    # Adaugă etichetă pentru obiectivul principal
    ax.text(main_objective['longitude'], main_objective['latitude'], 
            main_objective['name'], fontsize=8, ha='right')
    
    # Plot pentru vecini
    for neighbor in neighbors:
        ax.scatter(neighbor['longitude'], neighbor['latitude'], 
                  c='blue', s=100, alpha=0.6)
        
        # Adaugă etichetă pentru fiecare vecin
        ax.text(neighbor['longitude'], neighbor['latitude'], 
                neighbor['name'], fontsize=8, ha='right')
        
        # Adaugă linie între obiectivul principal și vecin
        ax.plot([main_objective['longitude'], neighbor['longitude']],
                [main_objective['latitude'], neighbor['latitude']],
                'k--', alpha=0.3)
    
    # Setează titlul și etichetele
    ax.set_title(f'Vecinii obiectivului {main_objective["name"]}')
    ax.set_xlabel('Longitudine')
    ax.set_ylabel('Latitudine')
    
    # Adaugă legenda
    ax.legend()
    
    # Ajustează layout-ul
    plt.tight_layout()
    
    # Salvează figura
    plt.savefig(f'neighbors_{objective_id}.png', dpi=300, bbox_inches='tight')
    print(f"\nFigura a fost salvată ca 'neighbors_{objective_id}.png'")
    
    # Afișează statistici despre vecini
    print("\nStatistici despre vecini:")
    print(f"Număr total de vecini: {len(neighbors)}")
    print("\nDetalii despre vecini:")
    for neighbor in neighbors:
        print(f"\nNume: {neighbor['name']}")
        print(f"Distanță: {neighbor['distance']:.2f} km")
        print(f"Rating mediu: {neighbor['averageRating']:.2f}" if neighbor['averageRating'] else "Rating: N/A")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Utilizare: python pot.py <objective_id>")
        sys.exit(1)
    
    objective_id = int(sys.argv[1])
    
    try:
        # Obține vecinii de la API
        neighbors = get_neighbors_from_api(objective_id)
        
        if not neighbors:
            print("Nu s-au găsit vecini pentru acest obiectiv")
            sys.exit(1)

        # Creează vizualizarea
        create_neighbors_visualization(objective_id, neighbors)

    except Exception as e:
        print(f"Eroare la generarea figurii: {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1) 