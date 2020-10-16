function b = householder(a,v,s,right)

if !right,
    b = a - s * v * (v' * a);
else
    b = a - s * (a * v) * v';
endif

endfunction