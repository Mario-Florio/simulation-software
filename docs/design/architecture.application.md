# Architecture — Application

**Stakeholder:** Software Developer.

**Concern:** How do we design application to remain extensible while supporting rapid development of core needs?

**Goal:** Allow for extensibility of UI by decoupling presentation from simulation business logic.

## Overview

Three-tiered architecture consisting of the following layers:

* Presentation
* Business logic
* Data management

This architecture supports extensibility of UI progressions in future installments, while allowing major efforts to be expended towards the main simulation engine in the current state of development.

---

## Presentation

Consists of:

* Views
* View models
* View conrtrollers

Examples:

* CLI
* Web app
* Graphs, Diagrams, and 2D models
* 3D Visualizer

---

## Business Logic

Core Simulation Engine logic.

> See [Architecture — Simulation Engine](architecture.simulation-engine.md) for architectural elaboration

---

## Data Management

Storage of simulation results, persistent state, and tranfer across application.

Storage may be:

* Local
* Database server

Separating *data management* from *business logic* allows data formats and protocols to evolve separately from simulation engine implementation.
