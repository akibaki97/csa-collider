function playground(L,A)
    
    n = length(A);
    elem = L;
    for i = 1:n,
        elem.next = lnode(A(i));
        elem = elem.next;
    endfor

endfunction

function y = fun1(x)
    y = x*x;
endfunction

function y = fun2(x)
    y = bitshift(x,2)
endfunction