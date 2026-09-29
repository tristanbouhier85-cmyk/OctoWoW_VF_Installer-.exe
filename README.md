# OctoWoW VF - installation manuelle

Ce depot contient uniquement les fichiers necessaires pour mettre OctoWoW en francais.

## Fichiers a telecharger

Telechargez ces deux fichiers :

```text
patch-6.mpq
speech.MPQ
```

## Installation

Fermez completement OctoWoW, le launcher et `WoW.exe`.

Ouvrez le dossier de votre jeu OctoWoW, par exemple :

```text
D:\OctoWoW
```

Ouvrez ensuite le dossier :

```text
D:\OctoWoW\Data
```

Copiez `patch-6.mpq` dans `Data` :

```text
D:\OctoWoW\Data\patch-6.mpq
```

Copiez `speech.MPQ` dans `Data`, puis renommez la copie en :

```text
D:\OctoWoW\Data\patch-7.mpq
```

Important : ne remplacez pas `Data\speech.MPQ`. Le fichier des voix francaises doit s'appeler `patch-7.mpq`.

## Activer le francais

Ouvrez le fichier :

```text
D:\OctoWoW\WTF\Config.wtf
```

Ajoutez ou remplacez la ligne `locale` par :

```text
SET locale "frFR"
```

Enregistrez le fichier.

## Cache

Si le jeu garde encore des textes en anglais, fermez le jeu puis renommez le dossier :

```text
D:\OctoWoW\WDB
```

en :

```text
WDB_sauvegarde
```

Ne supprimez pas le dossier : renommez-le seulement pour pouvoir le restaurer si besoin.

## Resume rapide

```text
patch-6.mpq -> D:\OctoWoW\Data\patch-6.mpq
speech.MPQ  -> D:\OctoWoW\Data\patch-7.mpq
Config.wtf  -> SET locale "frFR"
```

## Desinstallation

Fermez le jeu, puis supprimez uniquement ces deux fichiers :

```text
D:\OctoWoW\Data\patch-6.mpq
D:\OctoWoW\Data\patch-7.mpq
```

Si vous avez renomme `WDB`, vous pouvez remettre l'ancien dossier a son nom d'origine.

Ne supprimez pas les autres fichiers `patch-*.mpq` d'OctoWoW.
