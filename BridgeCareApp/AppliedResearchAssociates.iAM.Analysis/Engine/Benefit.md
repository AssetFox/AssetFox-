# The benefit of iAM!

## Notes

To calculate a cumulative benefit, you need:
- the potential treatment (with consequences & schedulings),
- the passive treatment,
- the section (with current attribute values & schedule),
- the current year of analysis,
- the attribute metadata (a lot of stuff),
- the performance curves,
- the benefit attribute,
- the benefit weighting attribute.

I think that's it.

## How benefit is calculated

The "benefit" $B_T$ of treatment $T$ is the "cumulative benefit" $C_T$ of $T$ minus the "cumulative benefit" $C_P$ of the passive treatment $P$:

$$
B_T = C_T - C_P
$$

Each $C$ is calculated independently over the "treatment outlook period" $N$, a fixed whole number of years starting from the current year.

To understand the calculation, it's helpful to visualize with a line plot of benefit vs time.
Cumulative benefit is the area under this curve.
Each point on the plot is a (year, benefit) pair.

Given a series $b = (b_0, b_1, \dots, b_N)$ of instantaneous benefit over outlook period $N$, the cumulative benefit is

$$
C = \sum_{y=1}^{N} b_{y-1} + \frac{b_y - b_{y-1}}{2}
$$

For a given outlook year $y$ and the corresponding benefit weight attribute value $w_y$, the corresponding benefit value is

$$
b_y = \max(0, b^+_y) \times w_y
$$

where, given the benefit limit $L$ and the current direct value $b_y'$ of the benefit attribute,

$$
b^+_y =
\begin{cases}
b_y' - L &\text{if the benefit attribute decreases with deterioration} \\
L - b_y' &\text{if the benefit attribute increases with deterioration}
\end{cases}
$$

If we compute the benefit attribute value series $b' = (b_0', b_1', \dots, b_N')$, we can use the above equations to compute $C$ and then $B_T$.

## How $b'$ is calculated

