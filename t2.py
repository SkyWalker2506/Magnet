import json
filepath = '/Users/musabkara/.claude/projects/-Users-musabkara-Documents-Unity-Magnet/a7119e62-0088-4b11-9298-acf8835a9db3.jsonl'
with open(filepath) as f:
    lines = f.readlines()
for i, line in enumerate(lines):
    obj = json.loads(line)
    if 'message' in obj:
        msg = obj['message']
        role = msg.get('role', 'unknown')
        content = msg.get('content', '')
        if isinstance(content, list):
            for c in content:
                if isinstance(c, dict):
                    ctype = c.get('type','')
                    if ctype == 'tool_use':
                        print('=== L'+str(i+1)+' ['+role+'] TOOL: '+c.get('name','')+'===')
                        print(str(c.get('input',{}))[:400])
                        print()
                    elif ctype == 'text':
                        txt = c.get('text','')
                        if txt.strip():
                            print('=== L'+str(i+1)+' ['+role+'] TEXT ===')
                            print(txt[:600])
                            print()
        elif isinstance(content, str) and content.strip():
            print('=== L'+str(i+1)+' ['+role+'] TEXT ===')
            print(content[:600])
            print()
