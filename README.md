Monte Carlo Simulation

Authors:

- Przemysław Małecki (entire WPF part of the project)
- Ewelina Cybula (entire C++ code)


SUMMARY:

  This project is academic final project created in WPF, meant to simulate magnetic domains in a square of initial size, and in initial temperature. The WPF framework was used for creating a window which allows user for setting initial conditions, like size of a square, temperature, duration of simulation, and period of saving intermediate step, stored in MonteCarloStep class. After simulation meet its duration, it can be continued for new duration and new period of saving steps. During each iteration effective magnetism is calculated, and stored within program, so it can be saved to a file for later use.

STUFF USED IN PROJECT:

-> WPF and C# - simple window with default styles, buttons, text inputs with preview input validations, starting other programs (c++ code) with input arguments and capturing output data from those programs, basic interaction commands, the code was done in more functional programming paradigm
-> C++ - functional programming paradigm, implemented monte carlo algorithm, reading input arguments and printing calculated results for later capture

OVERALL WORK:

  The WPF application first captures and validates input, then sends it to c++ code (since project was done in 2020, both programs are packed together - will be reorganised later for better display of the project), which executes monte carlo algorithm and sends back to WPF the results. The communication was implemented as printing and capturing certain chars, and then based on their meaning - C# code captures further data and stores it according to the scheme. Depending on initial size and duration, the simulation can last long period of time. And the received configurations of domains can be seen after simulation ends (asynchronous execution not included). The maps from next/previous steps can be changed with up and down arrows.
