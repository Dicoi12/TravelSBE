import requests
import matplotlib.pyplot as plt
import seaborn as sns
import numpy as np
import pandas as pd
from mpl_toolkits.mplot3d import Axes3D
import urllib3

# Dezactivează avertismentele pentru certificatul SSL auto-semnat
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

def get_cluster_analysis():
    try:
        url = 'https://localhost:7100/api/ML/cluster-analysis'
        response = requests.get(url, verify=False)
        response.raise_for_status()
        return response.json()['result']
    except Exception as e:
        print(f"Eroare la obținerea analizei clusterizării: {e}")
        return None

def plot_silhouette_scores(scores):
    plt.figure(figsize=(10, 6))
    df = pd.DataFrame(scores)
    
    # Plot pentru scorurile Silhouette
    plt.bar(df['clusterId'], df['score'])
    plt.title('Scoruri Silhouette per Cluster')
    plt.xlabel('ID Cluster')
    plt.ylabel('Scor Silhouette')
    
    # Adaugă numărul de obiective deasupra fiecărei bare
    for i, score in enumerate(df['score']):
        plt.text(df['clusterId'][i], score, 
                f"n={df['objectiveCount'][i]}", 
                ha='center', va='bottom')
    
    plt.savefig('silhouette_scores.png')
    plt.close()

def plot_cluster_statistics(stats):
    # Crează un DataFrame pentru statistici
    df = pd.DataFrame(stats)
    
    # Plot pentru rating-ul mediu
    plt.figure(figsize=(10, 6))
    plt.bar(df['clusterId'], df['averageRating'])
    plt.title('Rating Mediu per Cluster')
    plt.xlabel('ID Cluster')
    plt.ylabel('Rating Mediu')
    plt.savefig('average_ratings.png')
    plt.close()
    
    # Plot pentru prețul mediu
    plt.figure(figsize=(10, 6))
    plt.bar(df['clusterId'], df['averagePrice'])
    plt.title('Preț Mediu per Cluster')
    plt.xlabel('ID Cluster')
    plt.ylabel('Preț Mediu')
    plt.savefig('average_prices.png')
    plt.close()
    
    # Plot pentru distribuția tipurilor
    plt.figure(figsize=(12, 8))
    type_data = []
    for stat in stats:
        for type_name, count in stat['typeDistribution'].items():
            type_data.append({
                'Cluster': stat['clusterId'],
                'Tip': type_name,
                'Count': count
            })
    
    type_df = pd.DataFrame(type_data)
    pivot_df = type_df.pivot(index='Cluster', columns='Tip', values='Count')
    pivot_df.plot(kind='bar', stacked=True)
    plt.title('Distribuția Tipurilor per Cluster')
    plt.xlabel('ID Cluster')
    plt.ylabel('Număr de Obiective')
    plt.legend(title='Tip', bbox_to_anchor=(1.05, 1), loc='upper left')
    plt.tight_layout()
    plt.savefig('type_distribution.png')
    plt.close()

def plot_similarity_matrix(matrix):
    plt.figure(figsize=(10, 8))
    # Convertim lista de liste într-un array numpy
    matrix_array = np.array(matrix)
    sns.heatmap(matrix_array, annot=True, cmap='YlOrRd', fmt='.2f')
    plt.title('Matricea de Similaritate între Clustere')
    plt.xlabel('Cluster ID')
    plt.ylabel('Cluster ID')
    plt.savefig('similarity_matrix.png')
    plt.close()

def main():
    analysis = get_cluster_analysis()
    if analysis:
        print("Generare vizualizări pentru analiza clusterizării...")
        
        # Generează toate vizualizările
        plot_silhouette_scores(analysis['silhouetteScores'])
        plot_cluster_statistics(analysis['clusterStatistics'])
        plot_similarity_matrix(analysis['similarityMatrix'])
        
        print("\nVizualizări generate cu succes!")
        print("Fișiere create:")
        print("- silhouette_scores.png")
        print("- average_ratings.png")
        print("- average_prices.png")
        print("- type_distribution.png")
        print("- similarity_matrix.png")
    else:
        print("Nu s-au putut obține datele pentru analiză.")

if __name__ == "__main__":
    main() 