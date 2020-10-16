function C = nice_compan(n,k)
    s = 1;
    r(1:k) = s*rand(k,1) + s*I*rand(k,1);
    r(1:k) = r(1:k) ./ abs(r(1:k));
    m = ceil(n/2);
    r(k+1:m) = s*rand(m-k,1) + s*I*rand(m-k,1);
    r(m+1:m+(m-k)) = conj(r(k+1:m));

    r = transpose(r);
    %[r,abs(r)]

    p = 1;
    for i = 1:2*m-k,
        p = conv(p,[1,-r(i)]);
    endfor
    
    C = compan(p);
    
endfunction