RELEASE_TIMESTAMP=`date +%Y-%m-%d`

RELEASE_INC_VERSION=`cat Updates/build_version.txt`
RELEASE_INC_VERSION=$((RELEASE_INC_VERSION+1))

RELEASE_VERSION="2.0.0.${RELEASE_INC_VERSION}"
RELEASE_NAME="${RELEASE_VERSION}_preview_${RELEASE_TIMESTAMP}"
RELEASE_CHANGEINFO=`cat changelog.txt`

RELEASE_FILE_NAME="duplicati-${RELEASE_NAME}"

GIT_STASH_NAME="auto-build-${RELEASE_TIMESTAMP}"

UPDATE_ZIP_URLS="http://updates.duplicati.com/preview/${RELEASE_FILE_NAME}.zip;http://alt.updates.duplicati.com/preview/${RELEASE_FILE_NAME}.zip"
UPDATE_MANIFEST_URLS="http://updates.duplicati.com/preview/latest.manifest;http://alt.updates.duplicati.com/preview/latest.manifest"
UPDATER_KEYFILE="/Users/kenneth/Dropbox/Privat/Duplicati-updater-release.key"
XBUILD=/usr/bin/xbuild

if [ "x${RELEASE_CHANGEINFO}" == "x" ]; then
    echo "No information in changeinfo file"
    exit 0
fi

echo -n "Enter keyfile password: "
read -s KEYFILE_PASSWORD
echo

git stash save "${GIT_STASH_NAME}"

echo "${RELEASE_NAME}" > "Duplicati/License/VersionTag.txt"
echo "${UPDATE_MANIFEST_URLS}" > "Duplicati/Library/AutoUpdater/AutoUpdateURL.txt"
cp "Updates/release_key.txt"  "Duplicati/Library/AutoUpdater/AutoUpdateSignKey.txt"

rm -rf "Duplicati/GUI/Duplicati.GUI.TrayIcon/bin/Release"

mono "BuildTools/UpdateVersionStamp/bin/Debug/UpdateVersionStamp.exe" --version="${RELEASE_VERSION}"
${XBUILD} /p:Configuration=Debug "BuildTools/AutoUpdateBuilder/AutoUpdateBuilder.sln"
${XBUILD} /p:Configuration=Release Duplicati.sln
BUILD_STATUS=$?

if [ "${BUILD_STATUS}" -ne 0 ]; then
    echo "Failed to build, xbuild gave ${BUILD_STATUS}, exiting"
    exit 4
fi

if [ ! -d "Updates/build" ]; then mkdir "Updates/build"; fi

UPDATE_SOURCE=Updates/build/preview_source-${RELEASE_VERSION}
UPDATE_TARGET=Updates/build/preview_target-${RELEASE_VERSION}

if [ -e "${UPDATE_SOURCE}" ]; then rm -rf "${UPDATE_SOURCE}"; fi
if [ -e "${UPDATE_TARGET}" ]; then rm -rf "${UPDATE_TARGET}"; fi

mkdir "${UPDATE_SOURCE}"
mkdir "${UPDATE_TARGET}"

cp -R Duplicati/GUI/Duplicati.GUI.TrayIcon/bin/Release/* "${UPDATE_SOURCE}"
cp -R Duplicati/Server/webroot "${UPDATE_SOURCE}"

if [ -e "${UPDATE_SOURCE}/control_dir" ]; then rm -rf "${UPDATE_SOURCE}/control_dir"; fi
if [ -e "${UPDATE_SOURCE}/Duplicati-server.sqlite" ]; then rm "${UPDATE_SOURCE}/Duplicati-server.sqlite"; fi
if [ -e "${UPDATE_SOURCE}/Duplicati.debug.log" ]; then rm "${UPDATE_SOURCE}/Duplicati.debug.log"; fi
if [ -e "${UPDATE_SOURCE}/updates" ]; then rm -rf "${UPDATE_SOURCE}/updates"; fi
rm -rf "${UPDATE_SOURCE}/"*.mdb;
rm -rf "${UPDATE_SOURCE}/"*.pdb;

echo
echo "Building signed package ..."

mono BuildTools/AutoUpdateBuilder/bin/Debug/AutoUpdateBuilder.exe --input="${UPDATE_SOURCE}" --output="${UPDATE_TARGET}" --keyfile="${UPDATER_KEYFILE}" --manifest=Updates/preview.manifest --changeinfo="${RELEASE_CHANGEINFO}" --displayname="${RELEASE_NAME}" --remoteurls="${UPDATE_ZIP_URLS}" --version="${RELEASE_VERSION}" --keyfile-password="$KEYFILE_PASSWORD"

echo "${RELEASE_INC_VERSION}" > "Updates/build_version.txt"

mv "${UPDATE_TARGET}/package.zip" "${UPDATE_TARGET}/latest.zip"
mv "${UPDATE_TARGET}/autoupdate.manifest" "${UPDATE_TARGET}/latest.manifest"
cp "${UPDATE_TARGET}/latest.zip" "${UPDATE_TARGET}/${RELEASE_FILE_NAME}.zip"
cp "${UPDATE_TARGET}/latest.manifest" "${UPDATE_TARGET}/${RELEASE_FILE_NAME}.manifest"

mono BuildTools/UpdateVersionStamp/bin/Debug/UpdateVersionStamp.exe --version="2.0.0.7"

git add "Updates/build_version.txt"
git commit -m "Version bump to v${RELEASE_VERSION}-${RELEASE_NAME}"
git tag "v${RELEASE_VERSION}-${RELEASE_NAME}"

echo
echo "Built PREVIEW version: ${RELEASE_VERSION} - ${RELEASE_NAME}"
echo "    in folder: ${UPDATE_TARGET}"


