function V = random_convex_polygon(c,n)
    X = zeros(n,2);
    for i = 1:n,
        phi = rand * 2 * pi;
        r = (rand + 1) * 2;
        X(i,:) = [c(1) + r*cos(phi), c(2) + r*sin(phi)];
    endfor

    E = convhull(X);
    m = length(E);
    V = zeros(m,2);
    for i = 1:m,
        V(i,:) = X(E(i),:);
    endfor
end
