function [v,W,AW,BW] = gjk(A,B)
    % global DEBUG;
    % if exist('DEBUG'),
    %     DEBUG__ = DEBUG
    % else
    %     DEBUG__ = true
    % endif

    DEBUG__ = true;

    if DEBUG__,
        figure(1);
        axis equal;
        plot_polygon(A);
        plot_polygon(B);

        figure(2);
        axis equal;
        CSO = cso(A,B);
        plot_polygon(CSO);
        waitforbuttonpress;
    endif

    % space dimension
    n = columns(A);
    % starting point: can be any point from A - B
    v = A(1,:) - B(1,:);

    % utils
    W = zeros(n+1,n);                           % vec array, holds the simplex vertices
    AW = zeros(n+1,n);                          % vec array, holds the ... from A
    BW = zeros(n+1,n);                          % vec array, holds the ... from B
    y = 0;                                      % bitset representation of the current simplex
    global max_vertex;                          % maximum number of vertices in the simplex
    max_vertex = n+1;
    delta = zeros(2^max_vertex-1, max_vertex);  % matrix, holds the determinants for johnson's distance algorithm
    d = zeros(max_vertex,max_vertex);           % array holds the differences for johson's distance algorithm
    epsTol = eps * 1e10;
    lambda = zeros(max_vertex,1);               % the baricentric coordinates of the closest point

    while true,

        % support mapping of A - B in the direction of -v
        aw = support_mapping2d(A,-v);
        bw = support_mapping2d(B,v);
        w = aw - bw;

        % check if v is close enough to \nu(A - B)
        if is_in(W,w) || dot(v,v) - dot(v,w) <= eps^2 * dot(v,v),
            if DEBUG__,
                printf("v is close enough\n");
                figure(2); hold on;
                plot(v(1),v(2),"g*");

                figure(1); hold on;
                plot([a(1),b(1)],[a(2),b(2)],'g');
            endif

            break;
        endif

        [W,AW,BW,wbit] = prepare_simplex(W,AW,BW,y,w,aw,bw);

        % let W the smallest X \subseteq Y, such that v is in conv(X)
        [y,v,lambda, delta] = johnsons_distance(W,y,wbit,delta);

        if DEBUG__,
            W2 = zeros(0,n);
            first = true;
            firstInd = 0;

            bits = bitget(y,1:max_vertex);
            W2 = W(bits,:);
            W2 = [W2;W2(1,:)];

            figure(2);
            plot_polygon(W2);
            hold on;
            plot(v(1),v(2),"ro");

            a = b = zeros(1,n);
            for j = 1:max_vertex,
                a += lambda(j) * AW(j,:);
                b += lambda(j) * BW(j,:);
            endfor

            figure(1);
            hold on;
            plot(a(1),a(2),'r*');
            plot(b(1),b(2), 'b*');

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
                plot(v(1),v(2),"g*");
            endif

            break;
        endif
    endwhile

    bits = bitget(y,1:max_vertex);
    W = W(bits,:);
    AW = AW(bits,:);
    BW = BW(bits,:);

    if DEBUG__,
        figure(2);
        plot_polygon([W;W(1,:)]);
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
