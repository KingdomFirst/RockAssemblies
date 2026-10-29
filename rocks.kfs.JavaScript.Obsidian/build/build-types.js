const DeclarationBuilder = require("../../../Rock17/Rock.JavaScript.Obsidian/build/build-tools").DeclarationBuilder;

async function main() {
    const builder = new DeclarationBuilder();

    builder.arguments = ["--noEmit"];
    builder.importProject(builder.resolveProjectFile("./"), ref => ref.match(/[/\\\\]rocks.kfs.JavaScript.Obsidian[/\\\\]/) == null);

    const result = await builder.build();

    process.exit(result.success ? 0 : 1);
}

main();
