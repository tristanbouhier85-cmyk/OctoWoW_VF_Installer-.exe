# OctoWoW VF - installation manuelle

Ce depot contient uniquement les fichiers necessaires pour mettre OctoWoW en francais.

## A telecharger

Telechargez ces deux fichiers :

```text
patch-6.mpq
speech.MPQ
```

## Installation simple

### 1. Fermer le jeu

Fermez completement :

```text
OctoWoW
le launcher
WoW.exe
```

### 2. Copier les textes francais

Copiez :

```text
patch-6.mpq
```

dans :

```text
D:\OctoWoW\Data\patch-6.mpq
```

### 3. Copier les voix francaises

Copiez :

```text
speech.MPQ
```

dans :

```text
D:\OctoWoW\Data
```

Puis renommez cette copie en :

```text
patch-7.mpq
```

Le resultat doit etre :

```text
D:\OctoWoW\Data\patch-7.mpq
```

Important : ne remplacez pas `D:\OctoWoW\Data\speech.MPQ`. Les voix francaises doivent etre installees sous le nom `patch-7.mpq`.

### 4. Activer la langue francaise dans Config.wtf

Ouvrez ce fichier avec le Bloc-notes :

```text
D:\OctoWoW\WTF\Config.wtf
```

Cherchez une ligne qui commence par :

```text
SET locale
```

Remplacez-la par :

```text
SET locale "frFR"
```

Si la ligne n'existe pas, ajoutez simplement cette ligne a la fin du fichier :

```text
SET locale "frFR"
```

Enregistrez le fichier.

Cette etape est obligatoire : sans `SET locale "frFR"`, le jeu peut continuer a utiliser la langue anglaise.

### 5. Reinitialiser le cache si besoin

Si des textes restent en anglais, fermez le jeu puis renommez :

```text
D:\OctoWoW\WDB
```

en :

```text
WDB_sauvegarde
```

Ne supprimez pas le dossier `WDB`. Renommez-le seulement.

## Resume rapide

```text
patch-6.mpq -> D:\OctoWoW\Data\patch-6.mpq
speech.MPQ  -> D:\OctoWoW\Data\patch-7.mpq
Config.wtf  -> SET locale "frFR"
```

## Desinstallation

Fermez le jeu.

Supprimez uniquement :

```text
D:\OctoWoW\Data\patch-6.mpq
D:\OctoWoW\Data\patch-7.mpq
```

Si vous avez renomme `WDB`, vous pouvez remettre l'ancien dossier a son nom d'origine.

Ne supprimez pas les autres fichiers `patch-*.mpq` d'OctoWoW.
