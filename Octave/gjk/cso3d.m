function [W,F] = cso3d(U,V)

    n = rows(U);
    m = rows(V);

    X = zeros(n*m,3);
    for i = 1:n,
        for j = 1:m,
            X(j + (i-1)*m,:) = U(i,:) - V(j,:);
        endfor
    endfor

    F0 = convhull(X);
    k = rows(F0);
    indexMap = zeros(n*m,2);
    W = zeros(n*m,3);
    F = zeros(k,3);

    ind = 1;
    for i = 1:k,
        for j = 1:3,
            if indexMap(F0(i,j)) == 0,
                indexMap(F0(i,j)) = ind;
                W(ind++,:) = X(F0(i,j),:); 
            endif

            F(i,j) = indexMap(F0(i,j));
        endfor
    endfor

    W = W(1:ind-1,:);
end