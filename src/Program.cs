using Src.Core.Entities;
using Src.Core.Ports;
using Src.Adapters;

var simEngine = new SimulationEngine();

var A = new Variable("A", 0.0);
var B = new Variable("B", 1.0);
var C = new Variable("C", 2.0);

var ARef = simEngine.AddState(A);
var BRef = simEngine.AddState(B);
var CRef = simEngine.AddState(C);

simEngine.Run(10);

