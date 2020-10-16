## -*- texinfo -*-
## @deftp {Class} entry3d
##
## @end deftp

classdef entry3d < handle

    properties
        y       % the triangle's vertices: 3x3 array
        p       % p(i) - q(i) = y(i) (i = 1,2,3)
        q   
        v       % the closest point on the triangle to the origin
        d2      % distance from the origin
        lambda  % baricentric coordinates of v
        adj     % points to the adjacent triangle entries
        ind     % for each adjointing triangle adj(i), ind(i) is the index of the adjointing edge
        obs     % boolean flag to denote whether the triangle is visible from the new support point (short for obsolete)
    endproperties

methods

    function this = entry3d(y,p,q,v,d2,lambda)
        if nargin > 0,
        this.y = y;
        this.p = p;
        this.q = q;
        this.v = v;
        this.d2 = d2;
        this.lambda = lambda;
        endif
        this.obs = false;
        this.adj = {[],[],[]}; % empty references
    endfunction

    function plot_entry(this,col)
        plot3(
            [this.y(1,1),this.y(2,1),this.y(3,1),this.y(1,1)],
            [this.y(1,2),this.y(2,2),this.y(3,2),this.y(1,2)],
            [this.y(1,3),this.y(2,3),this.y(3,3),this.y(1,3)],
            'color',col);

        % plot3(this.v(1),this.v(2),this.v(3),'ko');

    endfunction

    function disp(this)
        printf("<object entry3d>\n\n");
        printf("y =\n%s\n",disp(this.y));
        printf("p =\n%s\n",disp(this.p));
        printf("q =\n%s\n",disp(this.q));
        printf("v =\n%s\n",disp(this.v));
        printf("d2 =\n%s\n",disp(this.d2));
        printf("lambda =\n%s\n",disp(this.lambda));
        printf("adj =\n%s\n",disp(this.adj));
        printf("ind =\n%s\n",disp(this.ind));
        printf("obs =\n%s\n",disp(this.obs));
    endfunction

    endmethods

endclassdef