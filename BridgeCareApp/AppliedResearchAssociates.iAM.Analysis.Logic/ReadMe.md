# Funding optimization

## Input

### Given

$$
\begin{align*}
    A(b,y) &= \text{money amount in budget $b$ for year $y$} \\
    &\ge 0 \\
    \\
    C(t) &= \text{money cost of treatment $t$} \\
    &\ge 0 \\
    \\
    F(y) &= \text{fraction of $C(t)$ to fund in year $y$} \\
    &\in [0, 1] \\
    \\
    P(b,t) &= \text{budget $b$ spending is permitted for treatment $t$ funding} \\
    &\in \{\text{true},\text{false}\} \\
    \\
    S_\text{carryover} &= \text{all unspent budget amounts carry over from year to year} \\
    &\in \{\text{true},\text{false}\} \\
    \\
    S_\text{multifund} &= \text{any treatment can be funded by multiple budgets} \\
    &\in \{\text{true},\text{false}\} \\
\end{align*}
$$

### Derived

$$
\begin{align*}
    K(t,y) &= \text{fractioned cost of treatment $t$ to fund in year $y$} \\
    &= C(t) \times F(y) \\
\end{align*}
$$

## Program

### Variables

$$
\begin{align*}
    x(b,t,y) &= \text{fraction of $K(t,y)$ to fund from budget $b$;} \\
    &\phantom{=} \text{defined only if $P(b,t)$} \\
    &\in
    \begin{cases}
        [0, 1] & \text{if $S_\text{multifund}$} \\
        \{0, 1\} & \text{otherwise} \\
    \end{cases} \\
\end{align*}
$$

### Constraints

$$
\begin{align*}
\end{align*}
$$

### Objective

$$
\begin{align*}
\end{align*}
$$

## Output

$$
\begin{align*}
\end{align*}
$$
