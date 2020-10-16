function [V,F] = clean_mesh(U,F)
    m = length(F);
    n = length(U);
    index_map = zeros(n,1);
    V = zeros(n,3);

    ind = 1;
    for i = 1:m,
        for j = 1:3,
            if index_map(F(i,j)) == 0,
                index_map(F(i,j)) = ind;
                V(ind++,:) = U(F(i,j),:);
            endif

            F(i,j) = index_map(F(i,j));
        endfor
    endfor

    V = V(1:ind-1,:);
endfunction