# Vizualizarea clusterelor pe grafic normalizat X/Y

## Scop

Clusterizarea obiectivelor turistice se face in spatiu cu 5 proprietati:

- longitudine (`Location.X`)
- latitudine (`Location.Y`)
- rating mediu
- pret numeric extras din `Pret`
- tipul obiectivului (`TypeId`)

Pentru prezentarea rezultatului in lucrarea de licenta, aceste 5 dimensiuni nu pot fi desenate direct intr-un grafic 2D obisnuit. De aceea, scriptul `ml_output/plot_clusters.py` genereaza un grafic principal numit `clusters_normalized_xy.png`, unde fiecare obiectiv este reprezentat prin coordonatele geografice normalizate, iar culoarea punctului indica clusterul calculat de K-Means pe toate cele 5 proprietati.

## Ce se afla pe axa X

Axa X reprezinta longitudinea normalizata si ponderata a obiectivului.

Formula folosita in `MLService.NormalizePoints` este:

```text
normX = ((longitudine - longitudineMinima) / (longitudineMaxima - longitudineMinima)) * LocationWeight
```

In configuratia curenta, `LocationWeight = 0.6`, deci valorile de pe axa X sunt in intervalul aproximativ `0..0.6`. O valoare mai mica inseamna ca obiectivul este mai spre vest in setul de date, iar o valoare mai mare inseamna ca obiectivul este mai spre est.

## Ce se afla pe axa Y

Axa Y reprezinta latitudinea normalizata si ponderata a obiectivului.

Formula folosita este:

```text
normY = ((latitudine - latitudineMinima) / (latitudineMaxima - latitudineMinima)) * LocationWeight
```

Si aici, cu `LocationWeight = 0.6`, valorile sunt in intervalul aproximativ `0..0.6`. O valoare mai mica inseamna ca obiectivul este mai spre sud in setul de date, iar o valoare mai mare inseamna ca obiectivul este mai spre nord.

## Cum s-a ajuns la rezultat

1. Backend-ul citeste obiectivele turistice din baza de date.
2. Pentru fiecare obiectiv construieste un punct cu 5 valori: X geografic, Y geografic, rating, pret si tip.
3. Fiecare valoare este normalizata prin min-max scaling, ca proprietatile cu unitati diferite sa poata fi comparate.
4. Valorile normalizate sunt ponderate cu greutatile din `appsettings.json`:
   - locatie: `0.6`
   - tip: `0.2`
   - rating: `0.1`
   - pret: `0.1`
5. Algoritmul Elbow alege numarul de clustere `K` pe baza valorii WCSS.
6. K-Means grupeaza obiectivele folosind toate cele 5 proprietati normalizate si ponderate.
7. `MLService` exporta rezultatul in `ml_output/cluster_data.json`.
8. `ml_output/plot_clusters.py` citeste fisierul JSON si deseneaza punctele pe axele `normX` si `normY`.

## Interpretarea graficului

In `clusters_normalized_xy.png`:

- fiecare punct este un obiectiv turistic
- culoarea punctului este clusterul atribuit de K-Means
- steaua marcheaza centroidul clusterului in spatiul normalizat X/Y
- axa X arata pozitia est-vest normalizata
- axa Y arata pozitia sud-nord normalizata

Important: pozitia punctului in grafic foloseste doar `normX` si `normY`, dar culoarea clusterului este rezultatul algoritmului aplicat pe toate cele 5 proprietati. Astfel, doua obiective apropiate pe harta normalizata pot ajunge in clustere diferite daca au rating, pret sau tip diferit.

## Grafice generate

Comanda:

```bash
python ml_output/plot_clusters.py ml_output/cluster_data.json --no-show
```

genereaza:

- `ml_output/clusters_normalized_xy.png` - graficul principal pentru prezentare
- `ml_output/clusters_geographic.png` - aceleasi clustere pe longitudine/latitudine reale
- `ml_output/clusters_pca.png` - proiectie PCA peste toate cele 5 proprietati, daca este instalat `scikit-learn`

Graficul PCA este util daca se doreste o proiectie 2D care tine cont matematic de toate cele 5 dimensiuni pe axe, dar axele sale (`PC1`, `PC2`) sunt combinatii liniare abstracte. Pentru prezentarea mai usor de explicat, graficul normalizat X/Y este mai clar: axele raman longitudine si latitudine normalizate.
