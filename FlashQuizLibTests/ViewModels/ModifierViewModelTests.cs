using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlashQuiz.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashQuiz.Models;

namespace FlashQuiz.ViewModels.Tests
{
    [TestClass()]
    public class ModifierViewModelTests
    {
        [TestMethod()]
        public void DeleteCardTest()
        {
            // Arrange
            var vm = new ModifierViewModel();
            Card card = new Card { Id = 1, Definition = "avoir", Terme = "haben" };
            Card card2 = new Card { Id = 2, Definition = "avoir", Terme = "haben" };

            // Act
            vm.Cards.Add(card);
            vm.Cards.Add(card2);
            vm.DeleteCard(card);


            // Assert
            Assert.AreEqual(vm.Cards.Count(), 1);
        }

        [TestMethod()]
        public void CreateCardTest()
        {
            // Arrange
            var vm = new ModifierViewModel();

            // Act
            vm.CreateCard();

            // Assert
            Assert.AreEqual(vm.Cards.Count(), 1);
        }
    }
}