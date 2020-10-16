[A,FA] = random_convex_polyhedron([-3,2,-1],20);
[B,FB] = random_convex_polyhedron([-1,-1,1],17);
AdjA = build_adj(A,FA);
AdjB = build_adj(B,FB);

% gjk3d(A,B,FA,FB,AdjA,AdjB)