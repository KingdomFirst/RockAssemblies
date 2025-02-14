[![Kingdom First Solutions](https://www.kingdomfirstsolutions.com/Content/ExternalSite/KFSArt/kingdomfirstlogo.png "Kingdom First Solutions")](https://www.kingdomfirstsolutions.com/rock)

# RockAssemblies

This is an open-source repository of [Kingdom First Solutions](https://www.kingdomfirstsolutions.com/rock) for all public Rock RMS Assemblies. Remember to check our [RockBlocks](https://github.com/KingdomFirst/RockBlocks) repo. Most of our code in this repository is targeted for RockRMS v16 and earlier. There are some changes for v16 that are breaking changes to prior versions, we will tag each previous version if you would like to use any of our code that was tested on prior versions. There are also hotfix branches available for early access versions in the event the code does not work properly in newer versions.

## Setup

The [RockAssemblies](https://github.com/KingdomFirst/RockAssemblies) and [RockBlocks](https://github.com/KingdomFirst/RockBlocks) repos are intended to be used together.

* `RockAssemblies` should be cloned to a sibling folder of the `RockWeb` folder (for reference relativity).
* `RockBlocks` should be cloned directly to `Plugins/rock_kfs`.

Example folder structure:

    .                            # Root GitHub or SDK folder
    ├── KFSRockAssemblies        # RockAssemblies Repo Root Folder
    │   ├── rocks.kfs...         # Specific KFS Assemblies
    │   └── ...
    ├── RockWeb                  # Core RockWeb folder from GitHub or SDK
    │   ├── Plugins              # Core Plugins folder from GitHub or SDK
    │   │   ├ rocks_kfs          # RockBlocks Repo Root Folder
    │   │   │   ├ Bulldozer      # Specific KFS Blocks
    │   │   │   └ ...
    │   │   └ ...
    │   └── ...
    └── ...

## Usage

Feel free to exercise your coding muscles and clone our repo.  We welcome pull requests if you'd like to contribute back!

If you prefer the one-click install method, we have also published several of our plugins to the [Rock Shop](https://www.rockrms.com/rockshop).

## History

We've worked with [Rock RMS](https://www.rockrms.com/) since its beginning and have helped hundreds of organizations with migrations (check out [Bulldozer](https://github.com/KingdomFirst/Bulldozer)), reports, and custom plugins.

## License

### Copyright 2024 by Kingdom First Solutions  

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0  

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

Any derivative works in this repository retain their original license for copyright purposes.
