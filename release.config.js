module.exports = {
  branches: [
    "master"
  ],
  plugins: [
    "@semantic-release/commit-analyzer",
    ["@semantic-release/release-notes-generator", {
      writerOpts: {
        footerPartial: `
{{#if noteGroups}}
{{#each noteGroups}}

### {{title}}

{{#each notes}}
* {{text}}
{{/each}}
{{/each}}
{{/if}}

You can install or upgrade by extracting all files from the zip attached to this release into a single directory or via [Chocolatey](https://chocolatey.org/packages/lessmsi).
        `
      }
    }],
    // github config docs: https://github.com/semantic-release/github
    ["@semantic-release/github", {
      "assets": [
        {"path": "src/.deploy/chocolateypackage/*.nupkg", "label": "Chocolatey Package"},
        {"path": "src/.deploy/*.zip", "label": "Zip of lessmsi application binaries"}
      ]
    }],
    ["@semantic-release/exec", {
      "verifyConditionsCmd": "src\\.build\\semantic-release-verify.cmd",
      "prepareCmd": "src\\.build\\semantic-release-prepare.cmd ${nextRelease.version}",
      "publishCmd": "src\\.build\\semantic-release-publish.cmd ${nextRelease.version}",
    }],    
  ]
};
