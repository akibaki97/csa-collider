function [y, v, lambda, delta] = johnsons_distance(W,y,w,delta)

    DEBUG__ = false;

    global max_vertex;

    % represents the whole simplex
    bits = bitor(y,w);

    % compute the determinants for all subsets containing w
    for x = 0:y,       
        xw = bitor(x,w);

        if !subseteq(xw,bits), continue;
        endif
        
        for i = 1:max_vertex,
            if !bitget(xw,i), continue;
            endif

            delta(xw,i) = calc_det(xw,i,delta,bits,W);           
        endfor
    endfor


    % find a valid subset
    for x = 0:y,
        % check if x is subset of y
        if !subseteq(x,y), continue;
        endif

        % check if x | w is a valid subset
        if is_valid(bitor(x,w), delta, bits), y = bitor(x,w); break;
        endif
    endfor

    % compute the coefficients for the closest vertex
    lambda = delta(y,:);
    detA = sum(lambda);
    lambda /= detA;

    % compute the closest vertex
    n = columns(W);
    v = zeros(1,n);
    for j = 1:max_vertex, v += lambda(j) * W(j,:);
    endfor

    v = W' * lambda';

end

function b = is_valid(x, delta, bits)

    global max_vertex;
    b = false;

    for j = 1:max_vertex,
        % check if it's in the simplex
        if !bitget(bits,j), continue;
        endif

        % check the two criterion
        if bitget(x,j),
            if delta(x,j) < 0, return;
            endif
        else
            if delta(bitor(x,bitshift(1,j-1)), j) > 0, return;
            endif
        endif
    endfor
    b = true;
    
endfunction


function delta_xi = calc_det(x,i,delta,bits,Y)

    global max_vertex;
    
    % early out: X contains only one vertex from the simplex
    if x && !bitand(x,x-1) && subseteq(x,bits),
        delta_xi = 1;
        return;
    endif

    delta_xi = 0;

    % z represents the set X\{y_i}
    z = bitset(x,i,0);

    k = 0;
    for j = 1:max_vertex,
        % check if j is in the index set of X
        if bitget(z,j),
            % let k be the smallest index of X
            if !k, k = j; endif

            % apply the recurion
            delta_xi += delta(z,j) * dot(Y(k,:)-Y(i,:),Y(j,:));
        endif
    endfor

endfunction


% tells if u is subset of v, or not.
function b = subseteq(u,v)
    b = bitand(u,v) == u;
endfunction

% do we even need this ?!
function b = contains(u,v)
    b = bitand(u,v) != 0;
endfunction