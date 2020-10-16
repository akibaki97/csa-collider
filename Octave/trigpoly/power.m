function lambda = power(A,x)
    
    N = 50;
    v = A*x;
    for k = 2:N,
        x = v;
        v = A*v;
    endfor

    lambda = dot(v,x) / dot(x,x);
endfunction