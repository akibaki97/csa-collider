function H = householder_mx(v,s)

    n = length(v);
    H = eye(n,n) - s * v * v';

endfunction