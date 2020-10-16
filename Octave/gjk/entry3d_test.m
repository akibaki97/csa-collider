p = rand(3,3);
q = rand(3,3);
y = p - q;
lambda = rand(3,1);
lambda(1) = 1 - lambda(2) - lambda(3);
v = y' * lambda;
d2 = dot(v,v);

entry = entry3d(y,p,q,v,d2,lambda);