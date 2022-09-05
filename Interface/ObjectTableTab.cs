using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;

using CottonCollector.Config;

namespace CottonCollector.Interface
{
    internal class ObjectTableTab : ConfigTab
    {
        public ObjectTableTab() : base("ObjectTable") { }

        public override void TabContent()
        {
            CottonCollectorConfig config = CottonCollectorPlugin.DalamudPluginInterface.GetPluginConfig() as CottonCollectorConfig ?? new CottonCollectorConfig();
            if (ImGui.BeginTabItem("Object Table"))
            {
                ObjectKindSelector(config);
                if (ImGui.BeginChild("ObjDataTable"))
                {
                    ObjectTable(config);
                    ImGui.EndChild();
                }

                ImGui.EndTabItem();
            }
        }

        private void ObjectKindSelector(CottonCollectorConfig config)
        {
            List<ObjectKind> objKindList = Enum.GetValues(typeof(ObjectKind)).Cast<ObjectKind>().ToList();
            int selectedIndex = objKindList.IndexOf(config.currKind);
            ImGui.Text($"kind size: {objKindList.Count}");
            if (ImGui.Combo("ObjectKindSelector", ref selectedIndex, Enum.GetNames(typeof(ObjectKind)), objKindList.Count))
            {
                config.currKind = objKindList[selectedIndex];
            }
        }

        private void ObjectTable(CottonCollectorConfig config)
        {
            if (ImGui.BeginTable("Objects", 4)) {
                // Table header
                ImGui.TableSetupColumn("Name");
                ImGui.TableSetupColumn("ObjectId");
                ImGui.TableSetupColumn("DataId");
                ImGui.TableSetupColumn("Pos");
                ImGui.TableHeadersRow();

                // Object table
                foreach (GameObject obj in CottonCollectorPlugin.ObjectTable)
                {
                    if (obj.ObjectKind != config.currKind) continue;
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text($"{obj.Name}");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.Text($"{obj.ObjectId}");
                    ImGui.TableSetColumnIndex(2);
                    ImGui.Text($"{obj.DataId}");
                    ImGui.TableSetColumnIndex(3);
                    ImGui.Text($"X:{obj.Position.X}, Y:{obj.Position.Y}, Z:{obj.Position.Z}");
                }
                ImGui.EndTable();
            }
        }


    }

}
