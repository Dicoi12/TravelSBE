# Algoritmul K‑Means — Explicație detaliată

Acest document oferă o descriere amplă (teoretică și practică) a algoritmului K‑Means pentru clustering (grupare nesupervizată).

## 1. Introducere
K‑Means este un algoritm iterativ folosit pentru partajarea unui set de puncte în `K` clustere astfel încât punctele din același cluster să fie cât mai asemănătoare între ele (în termeni de distanță), iar clusterele între ele cât mai diferite.

Scopul său este de a minimiza suma pătratelor distanțelor punctelor la centrul clustere‑lor (în engleză WCSS — within‑cluster sum of squares), adică funcția obiectiv:

J = sum_{i=1..K} sum_{x in C_i} ||x - mu_i||^2

unde `mu_i` este centroidul (media) clusterului `C_i`.

## 2. Formalizare
Date: set de n observații x_1, ..., x_n în R^d.
Parametru: numărul de clustere K (entier, 1 <= K <= n).
Ieșire: o partiție {C_1, ..., C_K} și centrii {mu_1, ..., mu_K} care minimizează J.

## 3. Pași ai algoritmului (varianta standard)
1. Inițializare: alege K centre inițiale (aleator sau folosit `kmeans++`).
2. Repetă până la convergență:
   a. Pasul de atribuire: pentru fiecare punct `x`, atribuie‑l clusterului al cărui centroid `mu_j` este cel mai apropiat (conform unei metrici de distanță, de ex. distanța Euclidiană).
   b. Pasul de actualizare: recalcularea centroidului fiecărui cluster ca media aritmetică a punctelor care i‑au fost atribuite: `mu_j = (1/|C_j|) * sum_{x in C_j} x`.

Convergența apare tipic când etichetele nu se mai schimbă sau când scăderea funcției obiectiv J este sub un prag.

## 4. Inițializare — de ce contează și metode
- Inițializare aleatorie: alege K puncte la întâmplare. Rapid, dar sensibil la rezultate locale.
- `kmeans++`: selectare inteligentă a inițialelor pentru a dispersa începuturile. Procedura:
  1. Alege primul centroid uniform din date.
  2. Pentru fiecare punct x calculează distanța D(x) la cel mai apropiat centroid ales.
  3. Alege un nou centroid cu probabilitate proporțională cu D(x)^2.
  4. Repetă până ai K centre.

`kmeans++` reduce riscul de soluții suboptimale și îmbunătățește stabilitatea.

## 5. Metrică de distanță
- Euclidiană (L2) este cea mai folosită (corelată direct cu minimizarea WCSS).
- Poți folosi și alte metrici (L1, cosine) dar atunci actualizarea centroidului și interpretarea obiectivului se schimbă (media nu e neapărat optimă pentru L1 sau cosine — ar trebui folosit mediană sau transformări).

## 6. Funcția obiectiv (WCSS) și interpretare
WCSS = sum_{j=1..K} sum_{x in C_j} ||x - mu_j||^2
- Scăderea WCSS indică o mai bună compactitate a clusterelor.
- WCSS scade monoton la fiecare iterație a K‑Means (algoritmul convergând către un minim local).

## 7. Criterii de oprire
- Nu se schimbă etichetele la o iterație.
- Schimbarea absolută sau procentuală a J < epsilon.
- Număr maxim de iterații atins.

## 8. Complexitate și cost computațional
- De bază: O(n * K * t * d)
  - n = număr de puncte
  - K = număr de clustere
  - t = număr de iterații
  - d = dimensiunea spațiului
- `kmeans++` adaugă un cost mic pentru inițializare, dar de obicei reduce t și îmbunătățește calitatea.

## 9. Avantaje și dezavantaje
Avantaje:
- Simplu, rapid pe seturi medii de date (în practică bun pentru multe aplicații).
- Implementare eficientă vectorizată.

Dezavantaje:
- Sensibil la inițializare (poate converga la minim local).
- Trebuie ales K în prealabil.
- Funcționează bine când clusterele sunt sferice și aproximativ de mărimi similare în termenii varianței; nu detectează bine clustere cu forme complexe.
- Sensibil la scara caracteristicilor; necesită normalizare (standardizare sau min‑max).

## 10. Probleme comune și soluții
- Caracteristici la scale diferite: scalează datele (StandardScaler sau MinMax) înainte.
- Puncte outlier: outlierii pot trage centroidul; fie elimini outlierii, fie folosești versiuni robuste (ex. `k‑medoids`).
- Un cluster devine gol (fără puncte): opțiuni:
  - Reinițializează centrul cu un punct aleator.
  - Pune centrul pe punctul cel mai îndepărtat.
- Date categorice: K‑Means (bazat pe medie numerică) nu e potrivit direct; folosește K‑Prototypes, K‑Modes sau codificări adecvate.

## 11. Alegerea lui K
- Metoda Elbow: trasați WCSS vs K și căutați 'cot' unde reducerea începe să se estompeze.
- Silhouette score: combină coeziunea și separarea; valoare între -1 și 1 (mai mare = mai bine).
- Gap statistic: compară WCSS cu distribuția aleatoare.
- Validare practică: criterii de business, interpretabilitate.

## 12. Variante și extensii
- Mini‑Batch K‑Means: actualizări folosind mini‑batch‑uri pentru scalare la volume mari.
- K‑Medoids: folosește medoidul (observația reală) în loc de medie — mai robust la outlieri.
- Gaussian Mixture Models (GMM): modelează clustere ca distribuții gaussiene (suprastructurare probabilistică).

## 13. Pseudocod
1. Initialize centroids mu_1..mu_K
2. repeat
   - For each x_i: assign label argmin_j ||x_i - mu_j||^2
   - For each j: mu_j = mean({x_i assigned to j})
   - compute J (opțional)
   - check stop criteria
3. until stop

## 14. Implementare minimală (Python, conceptual)

```python
# Pseudocod ilustrativ (nu optimizat)
import numpy as np

def kmeans(X, K, max_iter=100):
    n, d = X.shape
    # init: aleator
    idx = np.random.choice(n, K, replace=False)
    centroids = X[idx].astype(float)

    for _ in range(max_iter):
        # atribuire
        dists = np.linalg.norm(X[:, None, :] - centroids[None, :, :], axis=2)
        labels = np.argmin(dists, axis=1)

        # actualizare
        new_centroids = np.array([X[labels==j].mean(axis=0) if np.any(labels==j) else centroids[j]
                                  for j in range(K)])

        if np.allclose(new_centroids, centroids):
            break
        centroids = new_centroids

    return labels, centroids
```

Folosește `kmeans++` la inițializare și vectorizare pentru performanță în aplicații reale.

## 15. Sfaturi practice
- Normalizare: folosește `StandardScaler` sau `MinMaxScaler`.
- Rulează K‑Means de mai multe ori (n_init) și păstrează cel mai bun rezultat (cel cu WCSS minim) dacă folosești inițializare aleatorie.
- Pentru volume mari de date, alege `MiniBatchKMeans`.
- Vizualizează rezultatele (PCA / t‑SNE pentru d>2) ca să inspectezi consistența clusterelor.

---

Acest fișier explică conceptele principale; pot extinde secțiunile cu exemple practice (jupyter notebook), analiza complexității pentru implementări specifice sau insera un notebook cu vizualizări dacă dorești.