/* eslint-disable */
const path = require("path");
const { defineConfigs } = require("../../Rock15/Rock.JavaScript.Obsidian/Build/build-tools");

const workspacePath = path.resolve(__dirname);
const srcPath = path.join(workspacePath, "src");
const outPath = path.join(workspacePath, "dist");
const blocksPath = path.join(workspacePath, "..", "..", "Rock15", "RockWeb", "Plugins", "rocks_kfs", "Obsidian");

const configs = [
    ...defineConfigs(srcPath, outPath, {
        copy: blocksPath
    })
];

module.exports = configs;
