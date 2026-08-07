# Architecture — Simulation Engine (Business Logic Layer)

**Stakeholder:** Software Developer.

**Concern:** How do we design software architecture of simulation engine to meet requirements?

**Goal:** Simulate system state dynamics of home concept across various configurations and scenarios. Report results.

## Overview

The simulation software is employing a three-tier layered architecture, consisting of a presentation layer, business logic layer, and a data management layer. While the presentation and data management layer can be simplified at this early stage of development, the business logic layer requires a major upfront engineering effort. A simulation engine must be designed that is:

* operable at the current abstraction level (systems-level),
* extensible to further developmental levels (subsystem, component, and parts),
* exposes composable system agents, and
* observable, providing data for presentation consumption.

## Responsibilities

1. Store system state
2. Execute system functions
3. Expose configuration interface
4. Observe and report outcomes and interactions of interest

---

### Tasks

* Manage data
* Execute interactions
* Progress System
* Track events

#### Manage Data

* Store system constants and invariants
* Expose mutable system state to entities
* Control and prevent unintentional mutations

#### Execute Interactions

* Define transformation logic
* Control interaction dynamics and timing

#### Progress System

* Move system forward in simulation
* Not responsible for transformation or control flow

#### Track Events

* Observe system state change and interactions
* Define events of interest
* Log or otherwise report events of interest

---

### Architectural Entities

* System State
* Transformation Functions
* Control Functions
* System Clock
* Observer

#### System State
Any data which the system stores, tracks, manages, or otherwise acts on.

#### Transformation Functions
Any action which mutates state according to executable logic.

#### Control Functions
Any action which controls execution of tasks and actions.

#### System Clock
Oversee and orchestrate execution cadence and cycling.

#### Observer
Observe system state and interactions.

---

## Data, State, Flow

### Data Categories

* Static
* Dynamic

#### Static

* Configurations
* Constants / Invariants

#### Dynamic

* Mutable Variables

---

### State Ownership

* Shared System Globals
* State Managers

#### Shared System Globals
Any system state which is involved in macro-level processing.

#### State Managers
State that is managed locally via implemented system agents.

---

### Data Flow

1. Static State (configurations) : *Informs* -> Transformation Functions
2. Transformation Functions : *Mutates* -> Mutable Variables
3. Mutable Variables : *Informs* -> Transformation Functions

```text
+------------------------------------------+
| Shared System State                      |
| (Resources, System Config & Invariants)  |
+------------------------------------------+
    |           ^             ^
(Informs        |             |
 Production (Resource     (Resource
 & Usage)    Consumption)  Production)
    v           |             |
+------------------------------------------+
| Local State Managers                     |
| (Parameters, Operational state,          |
|  Production units,                       |
|  Operational Capacity Factors)           |
+------------------------------------------+
```
