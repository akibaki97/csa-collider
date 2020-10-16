function [v,W,AW,BW] = gjk3d(A,B,FA,FB,AdjA,AdjB)
    
    DEBUG__ = true;

    if DEBUG__,
        figure(1);
        axis equal;
        hold on;
        plot_polyhedron(A,FA,'b');
        plot_polyhedron(B,FB,'r');

        figure(2);
        axis equal;
        [CSO,FCSO] = cso3d(A,B);
        plot_polyhedron(CSO,FCSO,'k');
        waitforbuttonpress;
    endif

    % space dimension
    n = columns(A);
    if n != 3, error('wrong dim');
    endif

    % starting point: can be any point from A - B
    v = A(1,:) - B(1,:);

    % utils
    W = zeros(n+1,n);                           % vec array, holds the simplex vertices
    AW = zeros(n+1,n);                          % vec array, holds the ... from A
    BW = zeros(n+1,n);                          % vec array, holds the ... from B
    y = 0;                                      % bitset representation of the current simplex
    global max_vertex = n+1;                    % maximum number of vertices in the simplex
    delta = zeros(2^max_vertex-1, max_vertex);  % matrix, holds the determinants for johnson's distance algorithm
    d = zeros(max_vertex,max_vertex);           % array holds the differences for johson's distance algorithm
    epsTol = eps * 1e10;
    lambda = zeros(max_vertex,1);               % the baricentric coordinates of the closest point

    iterations = 0;
    while true,
        iterations++;

        % support mapping of A - B in the direction of -v
        aw = support_mapping3d(A,AdjA,-v);
        bw = support_mapping3d(B,AdjB,v);
        w = aw - bw;

        % check if v is close enough to \nu(A - B)
        if is_in(W,w) || dot(v,v) - dot(v,w) <= eps^2 * dot(v,v),
            if DEBUG__,
                printf("v is close enough\n");
                figure(2); hold on;
                plot3(v(1),v(2),v(3),"g*",'markersize',20);

                figure(1); hold on;
                plot3([a(1),b(1)],[a(2),b(2)],[a(3),b(3)],'g','markersize',20);
            endif

            return;
        endif

        [W,AW,BW,wbit] = prepare_simplex(W,AW,BW,y,w,aw,bw);

        % let W the smallest X \subseteq Y, such that v is in conv(X)
        [y,v,lambda, delta] = johnsons_distance(W,y,wbit,delta);

        if DEBUG__,
            bits = bitget(y,1:max_vertex);
            W2 = W(bits,:);

            figure(2);
            hold on;
            plot_simplex(W2);
                       
            plot3(v(1),v(2),v(3),"r.",'markersize',20);

            a = b = zeros(1,n);
            for j = 1:max_vertex,
                a += lambda(j) * AW(j,:);
                b += lambda(j) * BW(j,:);
            endfor

            figure(1);
            hold on;
            plot3(a(1),a(2),a(3),'g.','markersize',20);
            plot3(b(1),b(2),b(3),'g.','markersize',20);

            if(waitforbuttonpress != 0) DEBUG__ = false;
            endif
        endif

        % check if v is considerable as 0
        if y == 2^max_vertex-1 || norm(v) <= epsTol * max(diag(W * W')),
            v = zeros(1,n);

            if DEBUG__,
                printf("v is considerable as 0\n");
                figure(2);
                hold on;
                plot3(v(1),v(2),v(3),"g.",'markersize',20);
            endif

            break;
        endif

    endwhile

    bits = bitget(y,1:max_vertex);
    W = W(bits,:);
    AW = AW(bits,:);
    BW = BW(bits,:);

    if DEBUG__,
        % figure(2);
        % plot_polygon([W;W(1,:)]);
        printf("number of iterations: %d\n",iterations);
    endif

end

function b = is_in(A,x)

    n = rows(A);

    b = false;
    for i = 1:n,
        b = approx(A(i,:),x(:)');
        if b, return; endif
    endfor

endfunction

function [W,AW,BW,wbit] = prepare_simplex(W,AW,BW,y,w,a,b)

    % put w into the first position of W where y's bit is 0.
    for i = 1:4,
        if !bitget(y,i),
            wbit = bitshift(1,i-1);
            W(i,:) = w(:)';
            AW(i,:) = a(:)';
            BW(i,:) = b(:)';
            return;
        endif
    endfor

endfunction

function b = approx(a,b)
    b = dot(a-b,a-b) < eps^2;
endfunction
