# Funding optimization <!-- omit in toc -->

- [1. Input](#1-input)
  - [1.1. Given](#11-given)
  - [1.2. Derived](#12-derived)
  - [1.3. Internal](#13-internal)
- [2. Problem definition](#2-problem-definition)
  - [2.1. Variables](#21-variables)
    - [2.1.1. Related definitions](#211-related-definitions)
  - [2.2. Constraints](#22-constraints)
  - [2.3. Objective](#23-objective)
- [3. Procedure](#3-procedure)

## 1. Input

### 1.1. Given

$$
\begin{align*}
    A(b,y) &= \text{money amount in budget $b$ for year $y$} \\
    &\ge 0 \\
    \\
    C(t) &= \text{money cost of treatment $t$} \\
    &\ge 0 \\
    \\
    F(y) &= \text{fraction of each $C(t)$ to fund in year $y$} \\
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
    \\
    I(b) &= \text{numeric index of budget $b$ in the \bf{budget order}} \\
\end{align*}
$$

### 1.2. Derived

$$
\begin{align*}
    K(t,y) &= \text{fractioned cost of treatment $t$ to fund in year $y$} \\
    &= C(t) \times F(y) \\
\end{align*}
$$

### 1.3. Internal

$$
\begin{align*}
    M(b) &= \text{optimized maximal total spending from budget $b \in B_\text{max}$} \\
    B_\text{max} &= \text{set of budgets for which $M$ is defined} \\
\end{align*}
$$

## 2. Problem definition

### 2.1. Variables

$$
\begin{align*}
    x(b,t,y) &= \text{fraction of $K(t,y)$ to fund using budget $b$;} \\
    &\phantom{=} \text{defined only if $P(b,t)$} \\
    &\in
    \begin{cases}
        [0, 1] & \text{if $S_\text{multifund}$} \\
        \{0, 1\} & \text{otherwise} \\
    \end{cases} \\
\end{align*}
$$

#### 2.1.1. Related definitions

$$
\begin{align*}
    a(b,t,y) &= \text{money allocated from budget $b$ to treatment $t$ in year $y$} \\
    &= x(b,t,y) \times K(t,y) \\
    \\
    T(b) &= \text{total spending from budget $b$ over all treatments and years} \\
    &= \sum_{t,y} a(b,t,y) \\
\end{align*}
$$

### 2.2. Constraints

$$
\begin{align*}
    \forall\ t,y &: \textstyle\sum_b x(b,t,y) = 1 \\
    \\
    \forall\ b,y &:
    \begin{cases}
        \sum_t & \sum_{y'\,\le\,y} & a(b,t,y') & \le & \sum_{y'\,\le\,y} & A(b,y') & \text{if $S_\text{carryover}$} \\
        \sum_t & & a(b,t,y) & \le & & A(b,y) & \text{otherwise} \\
    \end{cases} \\
    \\
    \forall\ b \in B_\text{max} &: T(b) \ge M(b)
\end{align*}
$$

### 2.3. Objective

$$
\max_x\ \{ T(b')\ |\ b' = \argmin_{b\ \notin\ B_\text{max}} I(b) \}
$$

## 3. Procedure

1. Define $B_\text{max} \colonequals \empty$.
2. Build or update the problem definition w.r.t. $B_\text{max}$.
3. Maximize the objective value $T(b')$.
4. Add $b'$ to $B_\text{max}$.
5. Define $M(b') \colonequals T(b')$.
6. If $\exists\ b \notin B_\text{max}$, then go to 2, else stop.
