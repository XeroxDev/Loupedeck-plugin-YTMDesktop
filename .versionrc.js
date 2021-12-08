// .versionrc.js
const tracker = [
    {
        filename: './YTMDesktopPlugin/Properties/AssemblyInfo.cs',
        updater: require('./standard-version-updater/AssemblyInfo.js')
    },
    {
        filename: './package.json',
        type: 'json'
    },
    {
        filename: './LoupedeckPackage.yaml',
        type: 'yaml'
    }
]

module.exports = {
    sign: true,
    commitAll: true,
    bumpFiles: tracker,
    packageFiles: tracker
}