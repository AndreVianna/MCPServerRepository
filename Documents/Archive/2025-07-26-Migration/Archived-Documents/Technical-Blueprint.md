# MCP Registry — Consolidated Technical Blueprint (v4)

*(Merged with the "MCP Hub Comprehensive Proposal": same .NET 9 Aspire back‑end & NativeAOT CLI, **Blazor Web App portal**, mandatory namespacing, plus new **security‑council governance**, **progressive trust tiers**, and explicit **success metrics**. Manifest filename standardised to **mcps.json / mcps-lock.json**.)*

---

## 1 · Executive Summary

**MCP Registry** is a secure, cloud‑native hub for **Model Context Protocol servers**. Key stack:

* **Back‑end:** .NET 9 **Aspire** micro‑services, hosted on Kubernetes (cloud‑agnostic).
* **CLI (`mcpm`):** NativeAOT .NET 9 single‑file binaries using `System.CommandLine` 2.0 + `Spectre.Console`.
* **Portal:** **Blazor Web App** (server‑side prerender & WASM AOT) with Tailwind CSS + MudBlazor.
* **Security:** static analysis (Semgrep), dynamic sandbox (Firecracker), vet policy engine, progressive trust programme.

---

## 2 · Package & Manifest Standard

### 2.1 Files

* **mcps.json** — canonical manifest (alias: `mcp-manifest.json` accepted for backwards compatibility; registry rewrites to mcps.json).
* **mcps-lock.json** — signed deterministic lock.
* `.mcp` archive layout unchanged (`dist/`, README.md, LICENSE, optional src/).

### 2.2 Manifest Highlights

Same schema (runtime, capabilities, requiredConsents, dependencies) plus new field **`trustTier`** (registry‑generated: `unverified`, `community‑trusted`, `security‑audited`, `certified`).

---

## 3 · Secure Publish → Install Chain

Unchanged flow: `init ➜ publish (static + sandbox) ➜ fetch ➜ vet ➜ install`. CLI remains .NET NativeAOT.

---

## 4 · System Architecture (Aspire Micro‑services)

| Service | Responsibilities | Components |
|---------|------------------|------------|
| Gateway | YARP, auth, rate‑limit | .NET Aspire defaults |
| AuthService | JWT/OAuth2, MFA | ASP.NET Identity + Duende |
| RegistryService | Packages, versions, trust tier promotion | Minimal API + EF Core 9 |
| StorageService | Multi‑cloud blob (S3 or Azure Blob via Abstractions) | AWS SDK / Azure SDK |
| SearchService | OpenSearch + Qdrant vector | Elasticsearch client & Qdrant gRPC |
| SecurityAnalyzer | Semgrep + Firecracker sandbox | Worker Service |
| RuntimeMonitor | Optional managed execution metrics | Prometheus exporter |
| PortalService | Blazor Web App, MudBlazor, Tailwind | Hosted inside Aspire |

---

## 5 · Web Portal UX (Blazor)

* Search, Package Detail, Security Report Card, Org Dashboard.
* Real‑time SignalR push (scan results, download counts).
* Auth integrated with Gateway cookies.

---

## 6 · API Endpoints (unchanged)

Minimal APIs + HotChocolate GraphQL overlay.

---

## 7 · Security Framework

* **Static Scan** (Semgrep) & **Dynamic Sandbox** (Firecracker) on every publish.
* **Vet Policy Engine** enforces org policy before install.
* **Trust Tiers**:
  * `unverified` — default for new publishes.
  * `community‑trusted` — 100+ downloads, no failed scans.
  * `security‑audited` — passed manual review by Security Council.
  * `certified` — third‑party pentest + code review.
* **Security Council**: volunteer & staff group triaging reports, approving tier promotions, issuing CVE advisories.

---

## 8 · Governance & Community

* **Mandatory namespacing** (`@org/pkg`), trademark & inactivity dispute policy.
* **Progressive trust tiers** surfaced in UI & search ranking.
* Community forums (Discourse), code‑of‑conduct, contributor guide.

---

## 9 · Technology Stack Overview

| Layer | Choice |
|-------|--------|
| Back‑end | .NET 9 Aspire (C# 13) |
| Portal | Blazor Web App + MudBlazor + Tailwind |
| CLI | .NET 9 NativeAOT |
| DB | PostgreSQL + EF Core 9 |
| Search | OpenSearch + Qdrant |
| Cache | Redis |
| Storage | S3 *or* Azure Blob (configurable) |
| Infra | Kubernetes (AKS/EKS) |

---

## 10 · Success Metrics

| Category | KPI | Target after Year 1 |
|----------|-----|--------------------|
| Technical | API p95 latency | < 150 ms |
|  | Portal uptime | ≥ 99.9 % |
|  | Scan coverage | 100 % of publishes |
| Security | Sandbox false‑negatives | 0 critical escapes |
|  | Median time‑to‑patch vuln | < 48 h |
| Business | Monthly active developers | 2 000+ |
|  | Published servers | 1 500+ |
|  | Community‑trusted servers | 30 % of total |

---

## 11 · Phased Road‑map (unchanged timelines)

*Roadmap canvas already updated to Blazor Sprints; include tasks for trust‑tier promotion workflow & KPIs dashboards.*

---

## 12 · Next Steps

1. Align team on manifest rename policy (aliases allowed but registry stores `mcps.json`).
2. Draft Security Council charter; recruit initial 5 members.
3. Extend DB schema: `trust_tier` column, `security_council_reviews` table.
4. Add KPIs dashboard to Grafana (hook Prometheus + OpenSearch metrics).
5. Update roadmap & atomic to‑do canvases replacing storage‑specific tasks with multi‑cloud abstraction.
