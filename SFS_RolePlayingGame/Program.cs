using System;
using System.Collections.Generic;

namespace SFS_RolePlayingGame
{
    internal class Program
    {
        public static Random rand = new Random();
        static void Main(string[] args)
        {
            // 플레이어 객체 생성
            Player player;
            
            // 에필로그 스크립트 출력
            PrintScripts(epilogueScripts);
            Console.Clear();

            // 이름 입력 (람다식 사용해 조건 검사)
            string name = GetValidInput("[???] 모험을 하기에 앞서 용사님의 이름을 알려주세요", "내 이름은 ... ", s => !string.IsNullOrEmpty(s));
            Console.Clear();

            // 직업 입력 (람다식 사용해 조건 검사)
            string job = GetValidInput($"[???] {name}님 반가워요! 그나저나 지금 직업이 뭐라고 하셨었죠?\n[ 주인공의 직업은 '전사' 혹은 '마법사' 만 입력 가능합니다 ]", $"[{name}] 내 직업은 ... ", s => s == "전사" || s == "마법사");
            Console.Clear();

            switch (job)
            {
                case "전사":
                    player = new Warrior(name);
                    break;
                case "마법사":
                    player = new Mage(name);
                    break;
                default:
                    throw new Exception("잘못된 직업 입력");
            }

            Console.WriteLine("[ 당신은 같이 환생한 논산 육군훈련소 조교였던 병장 박아무개를 만났다. ]");
            NPC npc = new NPC("박아무개"); // '박아무개'라는 이름의 NPC를 생성

            int round = 1; // 시작 라운드 변수
            int maxRounds = 3; // 최대 라운드 변수
            bool gameWon = true;

            // 만약 현재 라운드가 최대 라운드보다 작거나 같다면
            // = 게임이 진행중이라면
            while (round <= maxRounds)
            {
                Console.WriteLine($"\n=== {round} 라운드 시작 ===");

                // 새로운 몬스터 리스트 생성
                List<Monster> monsters = CreateMonstersForRound(player.Level);

                // 팀 대 팀 전투 실시 후 승리 여부 boolean으로 반환
                bool roundWon = BattleRound(player, npc, monsters);

                // 만약 라운드에서 패배하였다면
                if (!roundWon)
                {
                    Console.WriteLine("[System] 당신은 패배하였습니다...");
                    gameWon = false;
                    break;
                }
                // 만약 라운드에서 승리하였다면
                else
                {
                    Console.WriteLine($"[System] {round} 라운드 승리!");
                    // 플레이어의 레벨을 상승시킨다
                    LevelUpPlayer(player);
                }

                // 라운드 상승
                round++;
            }

            // 모든 전투에서 승리하였다면 승리 메세지 출력
            Console.WriteLine(gameWon ? "[System] 축하합니다! 10라운드 전투에서 모두 승리했습니다!" : "[System] 게임 오버.");

            Console.WriteLine("게임을 종료하려면 아무 키나 누르세요...");
            Console.ReadKey();
        }

        // 내장 델리게이트 사용해 조건식에 따라 입력 검사하는 메서드
        static string GetValidInput(string message, string prompt, Func<string, bool> validator)
        {
            Console.WriteLine(message);
            Console.Write(prompt);
            string input = Console.ReadLine();

            while (!validator(input))
            {
                Console.Clear();
                Console.WriteLine("[System] 올바르지 않은 입력입니다! 다시 입력해주세요.");
                Console.Write(prompt);
                input = Console.ReadLine();
            }

            return input;
        }


        // 매 라운드마다 새로운 몬스터 객체 리스트 생성하는 메소드
        static List<Monster> CreateMonstersForRound(int playerLevel)
        {
            List<Monster> monsters = new List<Monster>();

            Orc orc = new Orc
            {
                Level = playerLevel,
                Hp = 100 + (playerLevel - 1) * 10,
                Power = 16 + (playerLevel - 1) * 3
            };
            monsters.Add(orc);

            Slime slime = new Slime
            {
                Level = playerLevel,
                Hp = 90 + (playerLevel - 1) * 8,
                Power = 20 + (playerLevel - 1) * 2
            };
            monsters.Add(slime);

            return monsters;
        }

        // 라운드 전투 진행 메서드
        static bool BattleRound(Player player, NPC npc, List<Monster> monsters)
        {
            while ((player.Hp > 0 || npc.Hp > 0) && monsters.Count > 0)
            {
                Console.WriteLine($"\n[Player HP: {player.Hp}, NPC HP: {npc.Hp}]");
                Console.WriteLine("몬스터 상태:");
                for (int i = 0; i < monsters.Count; i++)
                {
                    Console.WriteLine($"[{i}] {monsters[i].Name} (HP: {monsters[i].Hp}, Power: {monsters[i].Power})");
                }

                Console.Write("공격할 몬스터 번호 입력: ");
                if (!int.TryParse(Console.ReadLine(), out int targetIndex) || targetIndex < 0 || targetIndex >= monsters.Count)
                {
                    Console.WriteLine("잘못된 입력.");
                    continue;
                }

                player.Attack(monsters[targetIndex]);

                // NPC는 랜덤 몬스터를 공격
                npc.Attack(monsters[rand.Next(0,2)]);

                // 몬스터 체력이 0이 되면 사망 처리 (리스트에서 제거)
                if (monsters[targetIndex].Hp <= 0)
                {
                    Console.WriteLine($"{monsters[targetIndex].Name} 사망");
                    monsters.RemoveAt(targetIndex);
                }

                // 몬스터 리스트에 아직 몬스터가 남아 있다면
                if (monsters.Count > 0)
                {
                    Random rnd = new Random();
                    int npcTarget = rnd.Next(monsters.Count);
                    npc.Attack(monsters[npcTarget]);
                    if (monsters[npcTarget].Hp <= 0)
                    {
                        Console.WriteLine($"{monsters[npcTarget].Name} 사망");
                        monsters.RemoveAt(npcTarget);
                    }
                }

                /*foreach (var monster in monsters)
                {
                    // TODO : 몬스터 공격 대상 랜덤 지정으로 변경
                    if (player.Hp > 0)
                        monster.Attack(player);
                    else if (npc.Hp > 0)
                        monster.Attack(npc);
                }*/

                var targets = new List<Character>();
                if (player.Hp > 0) targets.Add(player);
                if (npc.Hp > 0) targets.Add(npc);

                foreach (var monster in monsters)
                {
                    if (targets.Count == 0) break;
                    var target = targets[rand.Next(targets.Count)];
                    monster.Attack(target);
                }

                Console.WriteLine("\n[Enter] 키를 누르면 다음 턴으로 진행합니다.");
                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

                Console.Clear();
            }

            return player.Hp > 0 || npc.Hp > 0;
        }


        // 플레이어 레벨업 메소드
        static void LevelUpPlayer(Player player)
        {
            int newHp = player.Hp + 30;
            int newPower = player.Power + 5;
            player.LevelUp(newHp, newPower);
        }

        // 에필로그 Script String 배열 변수
        private static readonly string[] epilogueScripts = {
            "Epilogue.",
            "현실에서 좌절한 나는 뜻밖에도 이세계에서 잊혀진 초능력을 가진 군주로 다시 태어났다.",
            "평화롭던 우리 마을은 옆 이종족 마을의 잔인한 침략으로 큰 위기에 빠졌다.",
            "가장 소중한 공주님이 적에게 납치되어, 마을 사람들의 희망은 점점 사라져갔다.",
            "그때, 현실의 기억을 고스란히 간직한 논산 육군훈련소 출신 병장 아무개가 조력자로 나타났다.",
            "강인한 훈련과 엄격한 규율로 다져진 그의 전투 능력은 우리에게 큰 힘이 되었다.",
            "두 사람은 함께 공주를 구하기 위해, 험난한 여정을 떠나기로 결심했다.",
            "이 전투가 마무리되면, 새로운 시대가 우리 앞에 펼쳐질 것이다.",
            "이제, 진정한 모험의 막이 오른다."
        };

        // string 배열을 차례대로 출력하는 Script 출력 메소드
        private static void PrintScripts(string[] scripts)
        {
            foreach (string script in scripts)
            {
                Console.WriteLine(script);
                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
            }
        }
    }
}
