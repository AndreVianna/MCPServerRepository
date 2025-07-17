# **MCPM: High-Level To-Do List (v4 Blueprint)**

## **Introduction**

This document translates the high-level "MCPM Detailed Implementation Roadmap" into an atomic, actionable to-do list for the development team. Each task is designed to be a small, manageable unit of work. **This version has been updated to reflect an Agile, sprint-based approach with just-in-time infrastructure provisioning.**

**How to Use This Document:**

* **Status:** Update from Pending to In Progress, Blocked, or Done.
* **Assigned To:** Assign each task to a team member or pod (e.g., Backend, Frontend, CLI).

## **Sprint 1: The "Walking Skeleton" \- Core API & Storage**

**Goal:** Establish the absolute minimum backend functionality. A developer, using an API client like Postman, can publish a package and retrieve its metadata and download URL.

| Task ID | Task Description | Status | Assigned To |
| :---- | :---- | :---- | :---- |
| 1.1 | Set up a monorepo or individual GitHub repositories for the .NET Aspire solution. | Pending |  |
| 1.2 | Create initial GitHub Actions workflow for building and running unit tests on every push to main. | Pending |  |
| 1.3 | **Infra:** Write Bicep scripts for and provision Azure Database for PostgreSQL. | Pending |  |
| 1.4 | **Infra:** Write Bicep scripts for and provision Azure Blob Storage containers. | Pending |  |
| 1.5 | **Auth:** Design final database schema for users, organizations, and api\_tokens using Entity Framework Core. | Pending |  |
| 1.6 | **Auth:** Implement /register and /login endpoints to issue JWTs. | Pending |  |
| 1.7 | **Auth:** Implement JWT validation middleware for protected API routes. | Pending |  |
| 1.8 | **Metadata:** Design final EF Core schema for packages and versions. | Pending |  |
| 1.9 | **Storage:** Design IStorageService interface and implement AzureBlobStorageService. | Pending |  |
| 1.10 | **API:** Implement a basic /publish endpoint that accepts metadata and a file, saving them to the DB and Blob Storage. | Pending |  |
| 1.11 | **API:** Implement a basic /packages/{owner}/{name} endpoint to retrieve package metadata. | Pending |  |

## **Sprint 2: The Foundational CLI**

**Goal:** Deliver a working CLI that can perform the core loop: login, init, publish, fetch, and install. This makes the system usable for developers for the first time.

| Task ID | Task Description | Status | Assigned To |
| :---- | :---- | :---- | :---- |
| 2.1 | Create a new .NET 9 Native AOT Console App project using System.CommandLine and Spectre.Console. | Pending |  |
| 2.2 | Implement secure, cross-platform storage for auth tokens. | Pending |  |
| 2.3 | Implement the mcpm login command to call the auth service and store the token. | Pending |  |
| 2.4 | Implement the mcpm init command with an interactive wizard. | Pending |  |
| 2.5 | Implement logic to bundle project files into a .mcp.tar.gz archive. | Pending |  |
| 2.6 | Implement logic to sign the archive using a user's local GPG/PGP key. | Pending |  |
| 2.7 | Implement the mcpm publish command to orchestrate bundling, signing, and API calls. | Pending |  |
| 2.8 | Implement the mcpm fetch command to download a package and verify its signature. | Pending |  |
| 2.9 | Implement the local caching mechanism at \~/.mcp/cache/. | Pending |  |
| 2.10 | Implement the mcpm install command to create a symlink from the cache. | Pending |  |
| 2.11 | Set up CI/CD pipeline in GitHub Actions to build and release cross-platform AOT binaries. | Pending |  |
| 2.12 | **Registry:** Add server-side validation to reject unsigned or un-namespaced packages. | Pending |  |

## **Sprint 3: The Public Portal (MVP)**

**Goal:** Launch a public, read-only website where anyone can browse and view packages. This creates the "front door" for the ecosystem.

| Task ID | Task Description | Status | Assigned To |
| :---- | :---- | :---- | :---- |
| 3.1 | **Infra:** Write Bicep scripts for and provision Azure Kubernetes Service (AKS). | Pending |  |
| 3.2 | **Infra:** Configure CI/CD pipeline (ArgoCD/Flux) to deploy the .NET Aspire app to AKS. | Pending |  |
| 3.3 | Set up the Blazor Web App project within the .NET Aspire solution. | Pending |  |
| 3.4 | Build reusable Razor components for buttons, inputs, cards, etc. | Pending |  |
| 3.5 | Implement a "Package List" page that shows recently updated packages. | Pending |  |
| 3.6 | Implement a "Package Detail" page that displays a package's metadata and README. | Pending |  |
| 3.7 | Implement a "User Profile" page that lists packages published by that user. | Pending |  |

## **Sprint 4: Foundational Security (vet & Static Analysis)**

**Goal:** Introduce the first layer of proactive, automated security checks.

| Task ID | Task Description | Status | Assigned To |
| :---- | :---- | :---- | :---- |
| 4.1 | **vet**: Implement the mcpm vet command logic to parse requiredConsents from mcps.json. | Pending |  |
| 4.2 | **vet**: Implement the policy engine to check consents against a local policy.json. | Pending |  |
| 4.3 | **Static Analysis:** Set up the Security Analysis Service project in the Aspire solution. | Pending |  |
| 4.4 | **Static Analysis:** Integrate a secret scanning tool (e.g., Gitleaks) into the service, triggered on publish. | Pending |  |
| 4.5 | **Static Analysis:** Integrate a malware signature scanning tool (e.g., ClamAV) into the service. | Pending |  |
| 4.6 | **API:** Create an endpoint to retrieve security scan results for a package. | Pending |  |
| 4.7 | **Web:** Display the static analysis results on the package's Blazor page. | Pending |  |

## **Sprint 5: Governance & Trust (MVP)**

**Goal:** Implement the initial framework for the Security Council and Progressive Trust Tiers.

| Task ID | Task Description | Status | Assigned To |
| :---- | :---- | :---- | :---- |
| 5.1 | **Docs:** Draft and publish the initial Security Council charter and Code of Conduct. | Pending |  |
| 5.2 | **Community:** Recruit the initial 5 members for the Security Council. | Pending |  |
| 5.3 | **DB:** Add trust\_tier column to the versions table and a security\_council\_reviews table to the EF Core schema. | Pending |  |
| 5.4 | **Backend:** Implement the automated promotion logic for a version to become community-trusted. | Pending |  |
| 5.5 | **Web:** Build an initial, simple dashboard for Security Council members to view pending packages and submit reviews. | Pending |  |
| 5.6 | **Web:** Display the current trust\_tier as a badge on package pages. | Pending |  |

## **Sprint 6: Advanced Search & Discovery**

**Goal:** Enhance the discovery experience with advanced search capabilities.

| Task ID | Task Description | Status | Assigned To |
| :---- | :---- | :---- | :---- |
| 6.1 | **Infra:** Provision an Elasticsearch cluster (e.g., Azure's native integration or a managed service). | Pending |  |
| 6.2 | Implement a data pipeline to index package metadata into Elasticsearch on publish. | Pending |  |
| 6.3 | **Web:** Enhance the search page to use Elasticsearch with faceted search filters. | Pending |  |
| 6.4 | **Infra:** Provision a Vector Database (e.g., Azure AI Search, Qdrant). | Pending |  |
| 6.5 | Implement an embedding pipeline to process package READMEs and descriptions. | Pending |  |
| 6.6 | **API:** Implement a new API endpoint for natural language (semantic) search queries. | Pending |  |
| 6.7 | **Web:** Add a semantic search bar to the portal's homepage. | Pending |  |

## **Sprint 7: V1.0 Polish & Launch**

**Goal:** Harden the platform, add key V1 features, and prepare for official launch.

| Task ID | Task Description | Status | Assigned To |
| :---- | :---- | :---- | :---- |
| 7.1 | **Dynamic Analysis:** Integrate a sandboxing tool (gVisor/Firecracker) into the Security Service. | Pending |  |
| 7.2 | **Dynamic Analysis:** Implement logic to monitor and compare runtime behavior against requiredConsents. | Pending |  |
| 7.3 | **Web:** Display dynamic analysis results on the security report card. | Pending |  |
| 7.4 | **API:** Implement a GraphQL overlay on top of the existing Minimal APIs using HotChocolate. | Pending |  |
| 7.5 | **Monitoring:** Set up Grafana dashboards to track KPIs (latency, uptime, etc.). | Pending |  |
| 7.6 | **CLI:** Implement mcpm run and mcpm test commands. | Pending |  |
| 7.7 | **Community:** Implement the full user reviews and ratings system. | Pending |  |
| 7.8 | **Community:** Implement the "Verified Maintainer" process and display badges. | Pending |  |
