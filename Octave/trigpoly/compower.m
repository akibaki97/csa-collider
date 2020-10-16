function lambda = compower(A,x,N)

    n = length(A);

    c = dot(A(1,:),x);
    v = [c;x(1:n-1)];

    for k = 2:N,
        x = v;
        c = dot(A(1,:),x);
        v = [c; v(1:n-1)];
    endfor

    lambda = dot(v,x) / dot(x,x);

endfunction