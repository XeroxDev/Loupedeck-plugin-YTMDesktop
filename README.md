# 1. Table of content
- [1. Table of content](#1-table-of-content)
- [2. Badges](#2-badges)
- [3. What is this Plugin?](#3-what-is-this-plugin)
- [4. Support / Feedback](#4-support--feedback)
- [5. Actions](#5-actions)
- [6. How to use it?](#6-how-to-use-it)
- [7. How to contribute?](#7-how-to-contribute)

# 2. Badges
[![Forks](https://img.shields.io/github/forks/XeroxDev/Loupedeck-plugin-YTMDesktop?color=blue&style=for-the-badge)](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/network/members)
[![Stars](https://img.shields.io/github/stars/XeroxDev/Loupedeck-plugin-YTMDesktop?color=yellow&style=for-the-badge)](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/stargazers)
[![Watchers](https://img.shields.io/github/watchers/XeroxDev/Loupedeck-plugin-YTMDesktop?color=lightgray&style=for-the-badge)](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/watchers)
[![Contributors](https://img.shields.io/github/contributors/XeroxDev/Loupedeck-plugin-YTMDesktop?color=green&style=for-the-badge)](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/graphs/contributors)

[![Issues](https://img.shields.io/github/issues/XeroxDev/Loupedeck-plugin-YTMDesktop?color=yellow&style=for-the-badge)](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/issues)
[![Issues closed](https://img.shields.io/github/issues-closed/XeroxDev/Loupedeck-plugin-YTMDesktop?color=yellow&style=for-the-badge)](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/issues?q=is%3Aissue+is%3Aclosed)

[![Issues-pr](https://img.shields.io/github/issues-pr/XeroxDev/Loupedeck-plugin-YTMDesktop?color=yellow&style=for-the-badge)](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/pulls)
[![Issues-pr closed](https://img.shields.io/github/issues-pr-closed/XeroxDev/Loupedeck-plugin-YTMDesktop?color=yellow&style=for-the-badge)](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/pulls?q=is%3Apr+is%3Aclosed)
[![PRs welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=for-the-badge)](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/compare)

<!-- [![Build](https://img.shields.io/github/workflow/status/XeroxDev/Loupedeck-plugin-YTMDesktop/CI-CD?style=for-the-badge)](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/actions?query=workflow%3A%22CI-CD%22) -->
[![Release](https://img.shields.io/github/release/XeroxDev/Loupedeck-plugin-YTMDesktop?color=black&style=for-the-badge)](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/releases)
[![Downloads](https://img.shields.io/github/downloads/XeroxDev/Loupedeck-plugin-YTMDesktop/total.svg?color=cyan&style=for-the-badge&logo=github)]()

[![Awesome Badges](https://img.shields.io/badge/badges-awesome-green?style=for-the-badge)](https://shields.io)

# 3. What is this Plugin?
This Loupedeck Plugin allows you to control the [YouTube Music Desktop App](https://github.com/ytmdesktop/ytmdesktop)

> [!NOTE]
> We only support version 2.x.x and above, if you are using an older version, please update to the latest version.

# 4. Support / Feedback
You found a bug? You have a feature request? I would love to hear about it [here](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/issues/new/choose) or click on the "Issues" tab here on the GitHub repositorie!

You can also join my discord [here](https://s.tswi.me/discord)

# 5. Actions

- Play / Pause Track
- Next Track
- Previous Track
- Like Track
- Dislike Track
- Volume Mute
- Volume Down
- Volume Up
- Track Info
  - Shows a scrolling text for album, title and author
  - Shows the thumbnail of the track
- Shuffle
- Repeat
  - NONE
  - ALL
  - ONE

# 6. How to use it?

> [!NOTE]
> This is just a simplified version, please visit the [official documentation](https://help.xeroxdev.de/en/stream-deck/ytmd/home#h-5-how-to-use-it) for a more detailed guide.

1. Install the [YouTube Music Desktop App](https://github.com/ytmdesktop/ytmdesktop).
2. Install the Plugin from [Releases](https://github.com/XeroxDev/Loupedeck-plugin-YTMDesktop/releases)
3. Add a settings profile action, fill out everything and click save.
4. Make sure the YouTube Music Desktop App and the Companion Server is running
   - Settings > Integration > Enable "Companion Server"
5. Turn on "enable companion authorization" under the Companion Server
6. Press the Profile button on your device
7. Compare the authorization code displaying by the plugin with the one displaying in the YouTube Music Desktop App and if they match, confirm the authorization in the YouTube Music Desktop App
8. If you've gotten no error and the plugin is now displayed in the "Authorized companions" list of YouTube Music Desktop, you can now use the plugin

# 7. How to contribute?

Just fork the repository and create PR's.

> [!NOTE]
> We're using [release-please](https://github.com/googleapis/release-please) to optimal release the library.
> release-please is following the [conventionalcommits](https://www.conventionalcommits.org) specification.