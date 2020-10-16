function [V,F] = random_convex_polyhedron(c,n)

    X = zeros(n,3);
    for i = 1:n,
        phi = rand * 2 * pi;
        theta = rand * pi;
        r = (rand + 1) * 2;
        X(i,1) = c(1) + r * sin(theta) * cos(phi);
        X(i,2) = c(2) + r * sin(theta) * sin(phi);
        X(i,3) = c(3) + r * cos(theta);
    endfor

    F = convhull(X);
    m = rows(F);
    indexMap = zeros(n,1);
    V = zeros(n,3);
    
    ind = 1;
    for i = 1:m
        for j = 1:3,
            if indexMap(F(i,j)) == 0,
                indexMap(F(i,j)) = ind;
                V(ind++,:) = X(F(i,j),:); 
            endif

            F(i,j) = indexMap(F(i,j));
        endfor
    endfor

    % change triangle order
    tmp = F(:,1);
    F(:,1) = F(:,2);
    F(:,2) = tmp;

    V = V(1:ind-1,:);
    
end