function A = rand_with_eigs(lambda)
    n = length(lambda);
    T = 10*rand(n,n);
    while abs(det(T)) < 1e-4,
        T = 10*rand(n,n);
    endwhile

    A = hess(inv(T) * diag(lambda) * T);
endfunction