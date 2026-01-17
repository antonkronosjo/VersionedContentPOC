import type { Language } from "../api/client";

type Param<K extends string> = K | `${K}?`;

class Route<P extends Record<string, unknown>> {
    constructor(
        public readonly route: string,
        public readonly params: readonly Param<keyof P & string>[]
    ) { }

    pattern(): string {
        return [
            this.route,
            ...this.params
        ].join("/:");
    }

    build(values: P): string {
        const parts = this.params
            .map(p => values[p.replace("?", "") as keyof P])
            .filter(v => v !== undefined)
            .map(String);

        return [this.route, ...parts].join("/");
    }
}


export const routes = {
    home: '/',
    select: '/select',
    create: new Route<{ contentType: string; language: Language }>(
        '/create',
        ['contentType', 'language']
    ),
    update: new Route<{ contentId: string; language: Language; versionId?: string }>(
        '/update',
        ['contentId', 'language', 'versionId?']
    ),
}
