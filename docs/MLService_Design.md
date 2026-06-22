# MLService — Design și Recomandări

Acest document explică modul în care a fost gândit și implementat serviciul `MLService` din proiectul TravelSBE, evidențiind funcționalitatea, avantajele și posibilele îmbunătățiri.

---

## 1. **Scopul Serviciului**
`MLService` este responsabil pentru:
- Antrenarea unui model de clustering pentru obiective turistice.
- Recomandarea de obiective similare unui utilizator.
- Vizualizarea clusterelor și analiza acestora.
- Salvarea datelor relevante pentru clustering (ex. WCSS, cluster centroids).

---

## 2. **Funcționalități Cheie**

### a. **Antrenarea modelului**
- **Metodă:** Algoritmul K-Means este utilizat pentru gruparea obiectivelor turistice în clustere.
- **Proces:**
  1. Normalizează datele (ex. locație, rating, preț, tip).
  2. Determină numărul optim de clustere `K` folosind metoda „Elbow”.
  3. Aplică K-Means pentru a atribui fiecare obiectiv unui cluster.
  4. Salvează rezultatele în baza de date și în fișiere JSON pentru vizualizare ulterioară.

### b. **Recomandări**
- **Cum funcționează:**
  1. Identifică clusterul obiectivului selectat și clusterele vecine.
  2. Calculează similaritatea între obiectivul selectat și celelalte obiective din clusterele vecine.
  3. Returnează o listă de obiective recomandate, ordonate după similaritate și distanță.
- **Metrici de similaritate:**
  - Locație (distanță geografică).
  - Rating mediu.
  - Preț.
  - Tipul obiectivului.

### c. **Vizualizare și analiză**
- **Date salvate:**
  - `elbow_data.json`: Datele WCSS pentru determinarea lui `K`.
  - `cluster_data.json`: Datele clusterelor și centroidelor.
- **Analize:**
  - Scoruri Silhouette pentru evaluarea calității clusterelor.
  - Statistici despre clustere (ex. distribuția tipurilor de obiective).
  - Matrice de similaritate între clustere.

---

## 3. **Aspecte Pozitive**

### a. **Arhitectură modulară**
- Funcționalitățile sunt bine separate în metode distincte (ex. `TrainModelAsync`, `GetRecommendedObjectivesAsync`).
- Interfața `IMLService` facilitează testarea și extensibilitatea.

### b. **Optimizări pentru performanță**
- Cache pentru a evita reantrenarea inutilă a modelului dacă datele nu s-au schimbat.
- Salvarea datelor în fișiere JSON pentru reutilizare și vizualizare.

### c. **Flexibilitate**
- Parametrii modelului (ex. `KMin`, `KMax`, ponderile pentru locație/rating/preț) sunt configurați din fișierul de configurare.
- Suport pentru clustere vecine și recomandări bazate pe similaritate.

### d. **Logare detaliată**
- Loguri clare pentru fiecare pas al procesului (ex. determinarea lui `K`, salvarea datelor).

---

## 4. **Posibile Îmbunătățiri**

### a. **Optimizarea algoritmului K-Means**
- **Inițializare:** Folosirea metodei `kmeans++` pentru alegerea centroidelor inițiale ar putea îmbunătăți stabilitatea rezultatelor.
- **Mini-batch K-Means:** Pentru seturi mari de date, implementarea unei variante mini-batch ar reduce timpul de antrenare.

### b. **Evaluarea clusterelor**
- **Silhouette Score:** Calcularea și utilizarea scorurilor Silhouette pentru a valida calitatea clusterelor.
- **Gap Statistic:** O metodă suplimentară pentru determinarea lui `K`.

### c. **Recomandări mai avansate**
- **Filtrare personalizată:** Permite utilizatorilor să prioritizeze anumite criterii (ex. rating mai mare, preț mai mic).
- **Model hibrid:** Combinarea clusteringului cu un model bazat pe învățare automată pentru recomandări mai precise.

### d. **Gestionarea outlierilor**
- Identificarea și tratarea outlierilor (ex. obiective cu locații sau prețuri extreme) pentru a evita influențarea negativă a centroidelor.

### e. **Scalabilitate**
- **Persistența modelului:** Salvarea modelului antrenat pentru reutilizare între sesiuni, reducând timpul de antrenare.
- **Distribuție:** Implementarea unui sistem distribuit pentru procesarea datelor mari.

### f. **Testare și monitorizare**
- Adăugarea de teste unitare pentru metodele principale (ex. `NormalizePoints`, `FindOptimalK`).
- Monitorizarea performanței modelului în timp real (ex. timpul de răspuns, calitatea recomandărilor).

---

## 5. **Concluzie**
`MLService` este o componentă bine structurată, cu funcționalități esențiale pentru clustering și recomandări. Cu toate acestea, există oportunități de îmbunătățire, în special în ceea ce privește optimizarea algoritmului, evaluarea clusterelor și scalabilitatea. Prin implementarea acestor sugestii, serviciul poate deveni mai robust, eficient și adaptabil la cerințe mai complexe.

---

## 6. **Formule Utilizate**

### a. **Funcția obiectiv pentru K-Means**
Funcția obiectiv minimizează suma pătratelor distanțelor punctelor la centroidul clusterului lor:

\(
WCSS = \sum_{i=1}^K \sum_{x \in C_i} \|x - \mu_i\|^2
\)

unde:
- \(K\): numărul de clustere.
- \(C_i\): punctele din clusterul \(i\).
- \(\mu_i\): centroidul clusterului \(i\).

### b. **Distanța Euclidiană**
Pentru calcularea distanței între două puncte \(p_1\) și \(p_2\):

\(
\text{Distanța} = \sqrt{(x_2 - x_1)^2 + (y_2 - y_1)^2 + (r_2 - r_1)^2 + (p_2 - p_1)^2 + (t_2 - t_1)^2}
\)

unde \(x, y, r, p, t\) sunt coordonatele normalizate (locație, rating, preț, tip).

### c. **Determinarea lui K (Metoda Elbow)**
Pentru a determina \(K\), metoda Elbow folosește distanța perpendiculară maximă între punctele \((k, WCSS)\) și linia dintre primul și ultimul punct:

\(
\text{Distanța} = \frac{|(WCSS_{\text{last}} - WCSS_{\text{first}}) \cdot k - (k_{\text{last}} - k_{\text{first}}) \cdot WCSS + k_{\text{last}} \cdot WCSS_{\text{first}} - WCSS_{\text{last}} \cdot k_{\text{first}}|}{\sqrt{(k_{\text{last}} - k_{\text{first}})^2 + (WCSS_{\text{last}} - WCSS_{\text{first}})^2}}
\)

### d. **Normalizarea Datelor**
Pentru fiecare caracteristică \(f\):

\(
\text{Valoare Normalizată} = \frac{f - f_{\text{min}}}{f_{\text{max}} - f_{\text{min}}} \cdot \text{Pondere}
\)

unde \(f_{\text{min}}\) și \(f_{\text{max}}\) sunt valorile minime și maxime ale caracteristicii, iar \(\text{Pondere}\) este greutatea atribuită caracteristicii respective.

### e. **Scorul de Similaritate**
Scorul de similaritate între două obiective \(o_1\) și \(o_2\):

\(
\text{Scor Similaritate} = (\text{Locație} \cdot w_{loc}) + (\text{Rating} \cdot w_{rating}) + (\text{Preț} \cdot w_{price}) + (\text{Tip} \cdot w_{type})
\)

unde \(w_{loc}, w_{rating}, w_{price}, w_{type}\) sunt ponderile configurate pentru fiecare caracteristică.

---

Dacă sunt necesare detalii suplimentare sau implementări specifice, acestea pot fi adăugate în secțiuni ulterioare.