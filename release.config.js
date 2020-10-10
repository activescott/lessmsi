module.exports = {
  branches: [
    "master",
    "chore/build-semantic-release-deploy",
  ],
  plugins: [
    "@semantic-release/commit-analyzer",
    "@semantic-release/release-notes-generator",
    ["@semantic-release/exec", {
      "verifyConditionsCmd": "src\\.build\\semantic-release-verify.cmd",
      "prepareCmd": "src\\.build\\semantic-release-prepare.cmd ${nextRelease.version}",
      "publishCmd": "src\\.build\\semantic-release-publish.cmd ${nextRelease.version}",
    }],
    // github config docs: https://github.com/semantic-release/github
    ["@semantic-release/github", {
      "assets": [
        {"path": "src\\.deploy\\chocolateypackage\\*.nupkg", "label": "NuGet distribution"},
        {"path": "src\\.deploy\\*.zip", "label": "ZIP distribution"}
      ]
    }],
  ]
};
