# OctoWoW VF Installer

Application WinForms autonome pour installer les textes et voix françaises de WoW 1.12.1.

OctoWoW VF traduit le contenu de base de WoW 1.12.1 et ajoute les voix françaises. Un seul EXE autonome installe, sauvegarde et désinstalle la VF. Compatible avec le launcher. Les ajouts propres à OctoWoW peuvent rester en anglais. Projet communautaire non officiel.

## Publication

```text
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

Le fichier publié est `bin/Release/net8.0-windows/win-x64/publish/OctoWoW_VF_Installer.exe`.
Les MPQ sont des ressources embarquées : aucun téléchargement ni client français n’est requis sur le poste final. Le binaire sera volumineux, car `speech.MPQ` fait environ 434 Mo.

Ressources incluses dans cette version :

```text
assets/patch-6.mpq  6 957 214 octets  SHA-256 686306BC0FDEB73CDD93B151F5050832396A72E3142FFF8CA3BEEC72E64115EA
assets/speech.MPQ 433 759 641 octets SHA-256 6792409A5AA32D192C97EDF9EC15AB26F6397293B350920A0FA80900A4B86331
```

L’application crée des sauvegardes horodatées, un fichier `OctoWoW_VF_installation.json` et le journal `OctoWoW_VF_installation.log` dans le client.

Avant diffusion, tester l’installation et la désinstallation sur une copie du client, puis générer un hash SHA-256 du binaire publié.
