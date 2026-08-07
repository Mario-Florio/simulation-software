# System Design — Simulation Engine

## System Overview

### System Participants

* Main Execution Loop
* Simulation Agents and Actors
  * Contain and Manage State
  * Dispatch transfromation functions (e.g., consumption of resources or effect on system environment)
  * Produce Simulation Resources
* Observability System
* Interface Adapters — serve as agents of control between external transformations and contained state

---

### Execution Model

* Tick-based
* Deterministic logic every tick
* Event-driven logic does not act on system state (only local observability state)
* Ordering:
  1. State is sampled
  2. Transformation functions operate
  3. Mutated state causes downstream transformational effects
* Queued *Simulation Agents Operations* execute once per tick

---

### Control Flow

1. System Environment and Configurations are set
2. Main Execution Cycle Begins
3. Simulation Agent Producers request needed Resources
4. Simulation Agents run Resource Production
5. Simulation Backup Agents run Production conditionally
6. Observation Logs any Events

---
