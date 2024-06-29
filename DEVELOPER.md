# Developer Guide

This guide provides instructions for releasing new versions of the software, creating pre-releases, and running smoke tests for pull requests.

## Releasing a New Version

To release a new version of the software, follow these steps:

1. **Increment the version number**: Decide the new version number following semantic versioning (e.g., `v1.0.1`).
2. **Tag the new version**: Create a new tag in for the Git repository, either locally or through the github UI. 
   ```sh
   git tag v1.0.1
   git push origin v1.0.1
3. **Push the tag**: This triggers the release workflow, which builds the project, generates the necessary artifacts, and creates a new GitHub release.

## Pre-release versions

A pre-release is built for every change pushed to the master branch. Clients will **not** automatically update to new pre-release versions. Clients will also **not** automatically update if they are running a pre-release version.

## Smoke-test 

Every PR will have a compile created as a smoke-test for the PR, but no release will be made. 