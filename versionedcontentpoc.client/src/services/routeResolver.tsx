import type { Language } from "../api/client";

class Route<P extends Record<string, unknown>> {
    constructor(
        public readonly route: string,
        public readonly params: (keyof P)[]
    ) { }

    build(values: P):string {
        return [this.route, ...this.params.map(p => String(values[p]))].join("/");
    }

    pattern(): string {
        return [this.route, ...this.params].join("/:");
    }
}

export const routes = {
    home: '/',
    select: '/select',
    create: new Route<{ contentType: string; language: Language }>(
        '/create',
        ['contentType', 'language']
    ),
    update: new Route<{ contentId: string; language: Language }>(
        '/update',
        ['contentId', 'language']
    ),
}
