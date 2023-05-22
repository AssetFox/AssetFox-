# Glossary document provided by Gregg Larson, 2020-03-26

This document specifies most of the behavior of the iAM analysis engine,
particularly the incarnation used in the desktop app ancestor of iAM (i.e.
RoadCare v3). The details of this document are not necessarily completely
up-to-date with the current iAM analysis engine, but most of it remains
accurate.

---

<u>Recommendations</u>

Replace all message with EventHandler. Publish the message through an
event handler. If a calling program wishes to get the message it can
handle the message.

Pass the data needed for an analysis into the program through a data
transfer object.

The determination of *Benefit/Cost/Remaining Life* can be done in
parallel. If there is processor overhead available compute
*Benefit/Cost/Remaining Life* using threading.

<u>Concepts</u>

Each *Section* can only have one *Treatment* for a given year.

Simulation should be completely asset type agnostic.

Only required *Attribute* is AGE.

Only required *Treatment* is No Treatment

Case sensitivity. String inputs into the model take into account case.

Null values in the data are replaced by default.

<u>Terminology</u>

*Analysis –* The main entry into the simulation. Values currently stored
in Simulations table. Inputs include *Optimization, Budget, Benefit
Variable, Weighting, Description, Jurisdiction Criteria, Benefit Limit,
Apply Multiple Feasible Costs.*

*Apply Multiple Feasible Costs –* If True, multiple *Costs* are filtered
by *Cost Criteria*, each should be applied. If this variable is False,
only the most expensive is applied.

*Area –* The unit area of the project. Can also be a count or size of an
asset (i.e. number of signs, length of pipe, etc). Currently a single
definition of area is allowed. ***Recommend: Area should be determined
using a criteria that allows multiple area equations. Can also be
accomplished by allowing user to select any attribute as Area. That
attribute could be a calculated field.***

*As Budgets Permit* ­– An analysis method in which the simulation spends
money according to *Priorities and Investments* by picking projects in
order of descending *Incremental Benefit/Cost, Maximum Benefit,
Remaining Life/Cost* or *Maximum Benefit.*

*Attribute –* An analysis variable in a simulation. May either be a
number (double) or a string. Each attribute has a date (year) associate.
All attributes that appear in any field (criteria, equation, combo box)
are loaded into the analysis at the start of the simulation. Only
attributes used in a simulation are loaded. Many attributes in the
network may not be used for a particular simulation. ***Recommend: Add a
third attribute type – DateTime. Currently this is modeled using the
number field (example years or days).***

*Ascending* – True (1) when the attribute gets larger with better
performance. An example of an ascending variable is PCI. PCI is 100 when
the pavement is new and goes towards 0 as it deteriorates. An example of
a false (0) attribute is IRI. IRI is somewhere near 60 for new pavement
and increases as the pavement deteriorates. A pavement with an IRI of
1000 is undrivable.

*Benefit Limit –* The limit below which *Benefit* does not accrue. For
example a bridge condition index may range from 0-10. Bridge condition
indexes below 2 have no value and should not be counted.

*Benefit Variable –* The *Attribute* that the analysis will optimize on
if *Incremental Benefit/Cost* or *Maximum Benefit* is selected. The
benefit variable can be a *Calculated Attribute*. Not used for
*Remaining Life* optimizations.

*Budget* – The budget a treatment can spend from. Budgets are defined
under Investments. A treatment can spend from multiple budgets. Budget
criteria (also defined under investments) determine if money can be
spent from a budget. ***Recommend: Currently budgets are handled as
strings. A budget object encapsulating budget behavior needs to be
created.***

*Budget Order* – Budgets are spent in the order they appear in the
*Investments*. For example, if there are three budgets Maintenance,
Rehabilitation, Reconstruction. All Maintenance *Projects* will be
considered first, then Rehabilitation and so on.

*Cash Flow –* Also known as Split Treatment. Cash flow is the *Criteria*
filtered splitting of *Projects* over set budget amounts into multiple
year projects. A *Project* is selected using the usual method, and then
is split into user determined amounts. Subsequent years of cash flow
*Project* are a type of *Committed Project.*

*Calculated Attributes* – Attributes that are calculated from other
attributes. The equation to calculate attributes is criteria selected.
After every update of consequences all calculated attributes are
recalculated.

*Committed Project-* A committed project occurs when a *Treatment* is
set for a given *Section* for a given year by user input or by means of
*Scheduled Project* or *Cash Flow (Split Treatment).* Committed projects
are not simply *Treatments* picked by the user. Each committed project’s
name, cost and consequence is unique. Committed projects are often
derived from a *Treatment*, but are independent. The simulation allows
the user to rename a committed project to a unique name. Committed
projects can have their own unique costs (or no costs at all). Committed
projects can have unique consequences, though in the case of multiple
instances of the same *Equation* the system only compiles it once.

*CodeDom* – Used to compile criteria and equations. Generates a .NET dll
for each equation or criteria. Greatly (several orders of magnitude)
improves performance of BridgeCare. Only run CodeDom once. If the
equation/criteria has been previously compiled and has not changed, load
DLL from binary. ***Recommend: Replace with Roslyn.***

*Criteria* – A Boolean expression evaluated against the current
attribute values. If the criteria evaluates true, the action the
criteria is attached to occurs (i.e. costs, consequences, deterioration
curve selection, feasibility). A blank or null *Criteria* evaluates to
true and act as a default value if no other criteria evaluates true.

-   *Budget criteria* – Determines if a treatment can spend from a
    budget. A blank or null budget criteria allows the treatment to
    spend from that budget.

-   *Calculated attribute criteria –* Determines which calculated
    attribute equations to use. Blank or null calculated attribute
    criteria are ignored if another calculated attribute criteria
    evaluates true.

-   *Consequence criteria –* Determines if a consequence should be
    applied to an attribute. Blank or null consequence criteria are
    ignored if another consequence criteria has evaluated true.

-   *Cost criteria* – Determines if given cost should be applied to the
    treatment. The total cost will be divided by the area of the asset
    to determine a cost per unit area. Blank or null cost criteria are
    evaluated even if other cost criteria evaluate true.

-   *Deficient criteria* – Determines if to include sections in
    deficient calculations. The value for deficient is compared to the
    value paired with the criteria. Whether the value is a floor or a
    ceiling depends on the *Ascending* Boolean.

-   *Deterioration criteria* – Determine if the deterioration equation
    should be applied to an attribute. Blank or null deterioration
    criteria are ignore if another deterioration criteria has evaluated
    true.

-   *Jurisdiction criteria –* Determines which sections are to be
    included in the simulation. Unlike all other criteria, jurisdiction
    criteria are not evaluated using the CodeDom compiler. Instead an
    non null or blank criteria is added to the select statement when
    selecting sections. The jurisdiction criteria is evaluated on
    non-roll forward data.

-   *Priority criteria* - Determines if a section is included in a given
    priority level. Sections can fall into multiple criteria levels. A
    blank or null criteria level allows all sections to spend from a
    given priority level.

-   *Remaining life criteria –* Determines if the limit for remaining
    life is evaluated. The limit is a floor or ceiling depending if the
    attribute is *Ascending*. A blank or null *Criteria* applies to all
    sections.

-   *Target criteria –* Determines if a section is included in
    calculation of a Target. Blank and null target criteria always apply
    to all sections determined by the jurisdiction criteria.

-   *Treatment Feasibility –* Determines if a given treatment can be
    selected. Blank and null treatment feasibility not considered.

*Default Value* – All *Attributes* have a default value to use if the
value is NULL.

*Deficient*- The value at which an attribute is considered deficient.
*Percent Deficient* allows that percentage of *Sections* weighted by
*Area.* To be deficient. The deficient level is either a floor or a
ceiling depending on the *Ascending* flag.

*Deterioration –* Any attribute (not including *Calculated Attributes*)
that changes over time. In the interface deterioration is called
*Performance.* ***Recommend: Change all references from Deterioration to
Performance.***

*Description ­*– Throughout the simulation a description field often
appears. Description is always optional and is used only for reporting
or user interface usability.

-   *Analysis Description ­­*– A description of the analysis being run.

-   *Deficient Name* – A description of the *Deficient Criteria, Value*
    and *Percent Deficient.*

-   *Investment Description* – A description of the *Investments* (web
    only).

-   *Performance Equation Name –* A description of the *Deterioration
    Criteria* and *Equation*.

-   *Target Name –* A description of the *Target* for the Result page.

-   *Treatment Description* – Description of the *Treatment*.

*Equations* – The calculation of a new value from current values (and
defaults in the simulation).

-   *Calculated Attribute equation –* An attribute calculated from other
    attributes. Calculated attributes are evaluated whenever attributes
    change (consequences or deteroriation).

-   *Consequence equation –* Calculate a new value of an attribute if
    selected by a criteria evaluating to true. Also allows simple set to
    values. ***Recommend:* Only compile unique consequence equations. If
    a consequence equations is the same across multiple treatments
    re-use the compiled dll.**

    -   \# - Entering a number. Sets the attribute to the value \#.

    -   +# - Adds the number to the current value.

    -   -# - Subtracts the number from the current value.

    -   +%# - Increase the current value by that percentage. Example,
        current value is 8, with a +10% consequence. New value is 8.8

    -   -%# - Decreases the current value by that percentage. Example,
        current value is 8, with a -50% consequence. The new value is 4.

-   *Committed Equations* – A special case of a *Consequence Equation.*
    Where *Consequence Equation* are tied to a specific *Treatment* each
    committed equation can be unique to that specific *Committed
    Project.* The system looks for committed equations that are the same
    across *Committed Projects* and compiles each unique equation only
    once (storing the result in a dictionary).

-   *Cost equation –* Determination of a cost. Non-exclusive. Multiple
    cost can be selected at the same time if Cumulative Cost flag is
    true.

-   *Deterioration* *curve* – How an attribute changes over time. Can
    either be an equation or piecewise. Deterioration curves are
    selected by criteria. System should warn if multiple valid
    deterioration curves are valid. In the case multiple are valid,
    chose the most conservative (worst performing). It is recommended
    that the \[AGE\] attribute be included. This allows the value to
    deteriorate with the passing of time. It is possible to use a time
    attribute other than \[AGE\], though this comes with a performance
    hit.

*Format –* A C# format string to apply to data when outputting after the
analysis. Internally, the simulation maintains all significant digits.
The format is applied only on output for reporting.

*Incremental Benefit –* The area under the deterioration curve after the
application of *Treatment* compared to the area under the
*Deterioration* curve if no treatment is selected.

*Incremental Benefit/Cost – Incremental Benefit* divided by unit cost of
a *Treatment.*

*Investments -* Stores data for start year, length of analysis, budget
definitions, budget orders, annual spending for budgets and budget
criteria.

*Minimum* – The minimum value an attribute can have. Currently a set
value. ***Recommend: Make this value criteria filtered.***

*Maximum* - The maximum value an attribute can have. Currently a set
value. ***Recommend: Make this value criteria filtered.***

*Maximum Benefit* – A method for selecting a *Treatment* pick according
to the amount of *Benefit* they give without consideration of the
*Cost.*

*Maximum Remaining Life* - A method for selecting a *Treatment* pick
according to the amount of *Remaining Life* they give without
consideration of the *Cost.*

*Percent Deficient* – For a *Deficient* level target the *Area* weighted
percentage that is allowed to be *Deficient*.

*Performance* – A synonym for *Deterioration*.

*Piecewise* – A type of deterioration equations where pairs of age/value
are given for an attribute. For years in between given pairs, a linear
interpolation is performed.

*Priority Level –* A *Criteria* filtered limit on the percentage of a
given *Budget* can be spent on a *Project.* If money allocated to a
priority is not spent it carries over to the lower priority. If the
*User Extra Funds* *Across Budgets* the extra money is given to the next
*Budget* in the *Budget Order.*

*Project –* Selecting a specific *Treatment* for a specific *Section.*

*Remaining Life –* The remaining life of a section is the number of
years (fractional) that a *Section* will take for one of its
*Attributes* to become reach its *Remain Life Limit.* Remaining life
cannot be negative. The attribute with the lowest remaining life
(multiple remaining life *Criteria* value pairs for a single *Attribute*
can be valid simultaneously) is the sections remaining life.

*Remaining Life/Cost* – A method for selecting *Treatments* ordered by
*Remaining Life* divided by the unit *Cost.*

*Roll-forward* – The process of applying *Deterioration* curves and
*Calculated Attributes* to bring historic data to the start year of the
simulation analysis.

*Scheduled­ ­*– A *Treatment* that is scheduled (*Committed)* in future
years as a result of a current *Project.* A scheduled treatment is
considered when calculated *Incremental Benefit* and *Remaining Life.*

*Section* – The logical unit in which simulation analysis are performed.
A section can be a single bridge, a length of road between two
mileposts, a group of assets (all lights in a parking lot), a building.
Sections are defined in Network Rollup and Network definition which is
outside the scope of the simulation.

*Supersedes -* A *Criteria* filtered list *Treatments* that the current
*Treatment* supersedes. For example, if *Section* can have both a minor
maintenance and a reconstruction *Project,* supersedes can say that the
minor maintenance will not be performed if a reconstruction is
*Feasible.* If a *Treatment* is listed in the supersedes of a *Feasible
Treatment,* it will remove that *Treatment* from consideration. Two
*Treatments* may mutually remove each other (there is not supersedes
precedent).

*Targets – A* simulation target for an attribute for *Sections* which
meet a *Criteria*. For example, the average condition index of all
*Sections* in a county must be greater than 8 (on a 0-10 scale).
*Sections* can be included in multiple targets. Targets are always
calculated when the analysis runs. In *Targets/Deficient Met* analysis
the analysis picks projects in order to meet targets.

*Targets/Deficient Met* – A method for picking *Projects* in order to
meet *Criteria* filtered *Targets.*

*Treatment –* A specific set of *Feasibility, Costs, Consequences,
Supercedes*

*Weighting* – An number *Attribute* that is multiplied by a *Benefit
Attribute* for determining *Benefit.* ***Recommend:* *Remove weighting
from analysis. The new functionality of Calculated Attributes deprecates
the need for weighting.***

*Year –* The year the *Priority, Target* or *Deficient* should apply
for.

*Years Same –* Number of years before the same *Treatment* can be
applied again to a *Section*. *Committed Projects* may ignore.

*Years Any –* Number of years before any *Treatment* can be applied to a
*Section*. *Committed Projects* may ignore.

<u>Important Variables</u>

*SimulationId* – The primary key of the Simulation. Each simulation has
a unique set of variables that determine the parameters of the
simulation.

*NetworkId –* The primary key for the BridgeCare network. A network is a
rolled up view of the raw data according to a specific set of rules. If
the underlying raw data changes the rolled up network does not change.
To update the data, the data must be rolled up again.

<u>Required Simulation Input Tables</u>

*SEGMENT_networkId_NSO –* Stores the *Attribute* values for each
*Section and Year.*

*SECTION_networkId –* Stores the *SECTION* definition of the network for
simulation

ATTRIBUTES\_ - Stores all the information about the *ATTRIBUTE*
including Type (number or string), *Minimum, Maximum, Format,
IsCalculated*

ATTRIBUTES_CALCULATED – Stores *Calculated Attribute* *Criteria* and
*Equations*

*SIMULATIONS* - Stores *Analysis* information

*INVESTMENTS*

*DETERIORATION*

*COMMITTED*

*TREATMENT*

*FEASIBILITY*

*COST*

*CONSEQUENCES*

*PRIORITY*

*PRIORITY_FUND*

*SPLIT_TREATMENT*

*SPLIT_TREATMENT_LIMITS*

*SCHEDULED*

*SUPERSEDES*

*TARGETS*

*DEFICIENT*

*REMAINING_LIFE_LIMITS*

<u>Created Simulation Output Tables</u>

Simulation created output tables are not used by the simulations and
simply store the results of the simulation. Simulation output tables are
loaded using bulk inserts from text files. ***Recommendation: Store this
information as data structures in some kind of NoSql type
implementation. No need for this data to be stored in the database.***

*BENEFIT_COST_networkId_simulationId* – Contains the calculated benefit
cost and remaining life for each feasible treatment.

*REPORT_networkId_simulationId –* Outputs the *Treatment* or *Committed
Project* for every year.

*TARGET_networkId_simulationId* – Outputs the *Targets* and *Deficient*
progress for every year.

*SIMULATION_networkId_simulationId\_# -* Output of each *Attribute*
value for each year of the Simulation. This table can get very wide, and
therefore is broken into multiple tables.

*REASONS_networkId_simulationId –* Output the reason each *Treatment*
was selected for a given year.

<u>Simulation Logic (much to add here)</u>

*SimulationMessaging* – Static class that holds global variables used in
the simualtions

Variables

-   

*CompileSimulation* – Main entry into the BridgeCare simulation. Loads
all of the necessary data from the database (equations, criteria,
attribute data), compiles the equations and criteria, loads defaults,
loads attribute data and rolls it forward.

Calls

-   `RunSimulation`

-   `DropPreviousSimulation`

-   `GetSimulationMethod`

-   `GetSimulationAttributes`

*RunSimulation* – Main execution loop of the analysis. Loops through all
years and all sections including in the analysis selecting projects
either as Budget Permits or Until Targets Met.
