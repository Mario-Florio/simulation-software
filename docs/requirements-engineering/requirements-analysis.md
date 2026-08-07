# Requirements Analysis

## Needs

* System Modeling
* Scenario Management
* Observation and Analysis
* Design Evaluation
* Extensibility
* Performance

> The following is a summary of the *Operation Analysis* (see [here](operational-analysis.md)).

---

### System Modeling

**Purpose:** Model high-level subsystem interactions while remaining abstracted from technology and implementation details.

This includes:

* System state evolution
* Subsystem consumption, production, and degradation
* Subsystem interactions and dynamics
* System-environment interactions
* Operational modes (e.g., storage, transport, installation, maintenance, logistics support, etc.)

---

### Scenario Management

**Purpose:** Create external constraints and impositions that control system initial state and behavior.

Scenarios are orthogonal to the system. They influence how the system functions, however, they are not the functions of the system itself.

---

### Observation and Analysis

**Purpose:** Observe and report relevant system metrics discovered during simulations.

This should inlcude *system performance parameters* and *measures of effectiveness*:

* Resource Production Capacity
* Inhabitant Resource Demand
* Overhead Cost
* Self-Sufficiency (*Feasibility*)
* Capital Dependency

As well as:

* Resource levels
* Subsystem states
* Failures
* Environmental conditions
* Task execution

---

### Design Evaluation

**Purpose:** Evaluate system design and architectural alternatives.

---

### Extensibility and Reusability

**Purpose:** Keep software open for extension and reuse for later systems depth (e.g., components, parts) and overall higer fidelity and precision.

---

### Performance

**Purpose:** Support timely simulation of many iterations and scenarios (target: seconds to minutes).

---

## Operational Objectives

* Model subsystem consumption, production, degradation, and side effects accurately for the current systems-level abstraction.
* Represent the system environment explicitly and allow it to participate in system dynamics.
* Observe system dynamics and report events relevant to *system performance parameters* and *measures of effectiveness*.
* Support extensibility and reusability, particularly as the simulation evolves from subsystem-level to component-level precision.
* Complete representative simulation studies within a target execution time of seconds to minutes.

---

## Problem Statement

The home system requires systems-level verification prior to moving into further conceptual development. Such verification must prove that internal system dynamics are compatible and cooperate overtime, in various scenarios, even when competing for resources. A feasible concept must not only remain cooperative and compatible, but provide resources for inhabitant demand.

Furthermore, it must be demonstrated that operational modes beyond normal functioning are feasible as well. This includes:

* Transportation
* Storage
* Installation
* Maintenance
* Logistic support

Such validation requires a significant and complex elaboration of intersystem dynamics. These intersystem dynamics must not only be articulted as a single discrete event, but a series of events which transpire and evolve over time. Furthermore, these unfolding intersystem dynamics must not be demonstrated one time, but many times, across varying scenarios, system configurations, and operational modes. Thus automating such process is deemed appropriate.

While simulation is the primary engineering task to be completed, the goal is not merely modeling intersystem dynamics. Rather, the dynamics must be simulated and observed to determine feasibility of design decisions. Thus intersystem dynamics must be interpreted in terms of relevant engineering metrics, including (but not limited too):

* System Performance Parameters
* Measures of Effectiveness
* Bottlenecks
* Utilization
* Fail modes
* etc.

Furthermore, while the current scope of the project only requires systems-level analysis — thus simulation does not require low-level precision — later developements may require high-fidelity modeling. Due to this, software design should remain extensible to such features, including the ability to extend:

* Value precision (e.g., floating point values)
* Hierarchical subsystem and component design
* Simulations including *hundreds* of agents with complex relationships
* etc.

---

## Current High-Level Requirements

### Must Haves

**Functional:**

* Simulation agents represent system members (e.g., system environment, susbsytems, components, etc.), modeling interactions such as:
  * Resource competition (e.g., allocation of finite resources)
  * Residual effects on system member performance (e.g., system environment -> subsystem output)
  * System state mutation (e.g., production, consumption, usage, etc.)
  * Hierarchical ownership (e.g., subsystem contains compoents, component contains parts, etc.)
* Value modeling is flexible and can support a variety of needs, such as:
  * Discrete, qualitative states / modes
  * Simple integer values
  * Floating point precision
* Observation hooks are composable and effortlessly dispatched to account for growing observability needs
* Reports (of relevant data and metrics) are decoupled from implementation and able to be accessed outside of simulation environment for further analysis

**Quality:**

* Performance remains within acceptable range for comprehensive simulations (target: seconds to minutes)

---

### Should Haves

**Functional:**

* Simulation agents are composable entities rather static hard-coded structures (*note:* early design could warrant static structures assuming extensibility is open for later deveplopment)
* UI provides basic report of relevant acquired data and metrics (*note:* if not feasible, reports stored separately in document is acceptable)

---

### Could Haves

**Functional:**

* UI provides graphs, 3D models, and visualizations of data

---

### Won't Haves

TBD.

---

## Further Elicitation

* Who will be using this system?
* What level of configuration should users possess?
  * Should users be able to design system agent models? Or should premade options be provided by software?
  * Should users be able to describe resource allocation and operational hierachy? Or are these things that are implicit in home system design?
* What attributes do scenarios possess?
  * Multipliers on performance? (e.g., Precipitation operates at 60% of capacity during Summer months)
* How may operational modes exist in practice?
  * Does transportation, storage, etc., need to account for commercial availability, temperature, etc.?
* Are simulations discrete-time, continuous-time, or hybrid?
* Are scenarios deterministic or probabilistic?
* What constitutes a simulation "tick"?
* What data must persist after execution?
* What constitutes a completed simulation?
* What constitutes simulation failure?
* What level of fidelity is required before subsystem-level analysis becomes component-level analysis?
* Who are the stakeholders?
  * What are their needs?
