function A = bulge_chase(A,m,ro)

    n = rows(A);

    if nargin < 3,
        ro = eig(A(n-m+1:n,n-m+1:n));
    endif

    x = A(:,1) - ro(1) * eye(n,1);
    x(1:2) = x(1:2) / norm(x(1:2));

    for k = 2:m,
        x(1:k+1) = A(1:k+1,1:k) * x(1:k) - ro(k) * x(1:k+1);
        x(1:k+1) = x(1:k+1) / norm(x(1:k+1));
    endfor

    [v,s,alpha] = get_householder(x(1:m+1));

    A(1:m+1,1:n) = A(1:m+1,1:n) - s * v * (v' * (A(1:m+1,1:n)));
    A(1:n,1:m+1) = A(1:n,1:m+1) - s * (A(1:n,1:m+1) * v) * v';

    for k = 2:n-1,
        bot = min(n,k+m);

        [v,s,alpha] = get_householder(A(k:bot,k-1));

        % apply from left
        A(k:bot,k-1:n) = A(k:bot,k-1:n) - s * v * (v' * A(k:bot,k-1:n));

        % apply from right
        A(1:n,k:bot) = A(1:n,k:bot) - s * (A(1:n,k:bot) * v) * v';
    endfor


endfunction

function [v,s,alpha] = get_householder(x)

    norm_x = norm(x);
    if norm_x < eps,
        v = x; s = 0;
        alpha = 0;
        return
    endif

    x = x / norm_x;
    abs_x1 = abs(x(1));
    if abs_x1 > 1e-8, c = x(1) / abs_x1;
    else, c = 1;
    endif

    v = x * conj(c);
    v(1) += 1;
    s = 1/v(1);
    alpha = -norm_x*c;

endfunction