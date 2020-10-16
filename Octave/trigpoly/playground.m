function y = playground(p,x)
  
    n = length(p)-1;
    y = p(1);

    for k = 2:n+1,
        y = p(k) + y * x;
    endfor

endfunction